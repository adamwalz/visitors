using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using MeshUtilities;
using SerializaahNS;

public abstract class PiecemakerWindowBase : EditorWindow 
{	
	[SerializeField]
	protected Piecemaker.Settings settings = new Piecemaker.Settings();
	
	Piecemaker.LevelData editingLevelData;
	Vector2 levelOverviewScrollPos;
	Vector2 scrollPos;
    string lastUsedPath = string.Empty;
	
	Piecemaker.PiecemakerCore core;

    void OnDestroy()
    {
        if (core != null)
        {
            core.Dispose();
            core = null;
        }
        if (settings != null)
            settings.SaveChanges();
        SaveSettings("LastSettings.piece");
    }

    void Update()
    {
        if (core != null)
        {
            if (core.IsFinished())
            {
                EditorUtility.ClearProgressBar();
                this.Repaint();
                core.ProcessExport();
                core.Dispose();
                core = null;
            }
            else
                core.CheckProcess();
            this.Repaint();
        }
    }
	
	void OnGUI()
	{
        if (settings != null)
            settings.Load(false);

        if (core != null)
            GUI.enabled = false;

		var estimatedSize = EstimateGUISize();

		scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), scrollPos, estimatedSize);
		
		var currentBox = new Rect(5, 5, 500, 0);
		currentBox = DrawMeshGUI(currentBox, false);
		currentBox.y += currentBox.height + 5;
		currentBox = DrawCutAreaGUI(currentBox, false);
		
		currentBox.height = currentBox.yMax - 5;
		currentBox.y = 5;
		currentBox.x += currentBox.width + 5;
		currentBox = DrawDestructionLevelOverview(currentBox, false);
		
		currentBox.width += currentBox.width + 5;
		currentBox.x = 5;
		currentBox.y += currentBox.height + 5;
		currentBox = DrawProcessingGUI(currentBox, false);
		
		GUI.EndScrollView();

        if (core != null)
        {
            GUI.enabled = true;

            if (!core.IsFinished())
            {
                GUI.Box(new Rect(position.width / 2 - 150, position.height / 2 - 10, 300, 20), "Processing..");

                var index = core.CurrentSliceIndex();
                var cnt = core.SliceCount();
                if (EditorUtility.DisplayCancelableProgressBar("Processing", string.Empty, index / (float)cnt))
                {
                    core.Dispose();
                    core = null;
                    EditorUtility.ClearProgressBar();
                }
            }
            else
            {
                EditorUtility.ClearProgressBar();
                GUI.Box(new Rect(position.width / 2 - 150, position.height / 2 - 10, 300, 20), "Saving..");
            }
        }
	}
	
	Rect EstimateGUISize()
	{
		var maxX = 0;
		var maxY = 0;
		var estimatedSize = new Rect(5, 5, 500, 0);
		estimatedSize = DrawMeshGUI(estimatedSize, true);
		CalcBounds(estimatedSize, ref maxX, ref maxY);
		
		estimatedSize.y += estimatedSize.height + 5;
		estimatedSize = DrawCutAreaGUI(estimatedSize, true);
		CalcBounds(estimatedSize, ref maxX, ref maxY);
		
		estimatedSize.height = estimatedSize.yMax - 5;
		estimatedSize.y = 5;
		estimatedSize.x += estimatedSize.width + 5;
		estimatedSize = DrawDestructionLevelOverview(estimatedSize, true);
		CalcBounds(estimatedSize, ref maxX, ref maxY);
		
		estimatedSize.width += estimatedSize.width + 5;
		estimatedSize.x = 5;
		estimatedSize.y += estimatedSize.height + 5;
		estimatedSize = DrawProcessingGUI(estimatedSize, true);
		CalcBounds(estimatedSize, ref maxX, ref maxY);
		
		return new Rect(0, 0, maxX, maxY);
	}
	
	void CalcBounds(Rect rect, ref int maxX, ref int maxY)
	{
		maxX = maxX > (int)rect.xMax ? maxX : (int)rect.xMax;
		maxY = maxY > (int)rect.yMax ? maxY : (int)rect.yMax;
	}
	
	Rect DrawMeshGUI(Rect currentBox, bool estimateSizeOnly)
	{
		currentBox.height = 330 + (settings.Materials != null ? settings.Materials.Length * 20 : 0);
		if (estimateSizeOnly)
			return currentBox;
		
		var currentPosition = currentBox;
		currentPosition.x += 20;
		currentPosition.y += 60;
		
		GUI.Box(currentBox, "Select the source Mesh you want to destruct. This mesh will be processed by Piecemaker and split into several shard meshes and prefabs, you can also select the material for each mesh part (submesh).");
		GUI.Label(new Rect(currentPosition.x, currentPosition.y, 150, 18), "Mesh:");
		var newMesh = EditorGUI.ObjectField(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), settings.Mesh, typeof(Mesh), false) as Mesh;
		DrawHelpSymbol(currentPosition.x, currentPosition.y, "The mesh you want to destruct. Theoretically you can use every mesh you want, but complex meshes could issue strange cut results so please refrain from using meshes with holes like a tube mesh (you can use a tube, but if you cut it along the inner tube direction the results may not be satisfying).");
		if (settings.Mesh != newMesh)
		{
			if (newMesh == null)
				settings.Materials = new Material[0];
			else
			{
				var subMeshCount = newMesh.subMeshCount;
				settings.Materials = new Material[subMeshCount];
			}
			settings.Mesh = newMesh;
		}
		
		if (settings.Materials != null && settings.Materials.Length > 0)
		{
            currentPosition.y += 20;

            GUI.Label(new Rect(currentPosition.x, currentPosition.y, 150, 18), "Material count:");
            var newMaterialCount = EditorGUI.IntField(new Rect(currentPosition.x + 150, currentPosition.y, 182, 18), settings.Materials.Length);
            if (newMaterialCount < 1)
                newMaterialCount = 1;
            if (newMaterialCount != settings.Materials.Length)
                System.Array.Resize(ref settings.Materials, newMaterialCount);

			for (int i = 0; i < settings.Materials.Length; ++i)
			{
                currentPosition.y += 20;
				GUI.Label(new Rect(currentPosition.x, currentPosition.y, 150, 18), string.Format("Material {0}:", (i+1).ToString()));
				DrawHelpSymbol(currentPosition.x, currentPosition.y, string.Format("Material used to render the {0}. submesh of each mesh.", i+1));
                settings.Materials[i] = EditorGUI.ObjectField(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), settings.Materials[i], typeof(Material), false) as Material;
			}
		}
		
		currentPosition.y += 30;
		GUI.Label(new Rect(currentPosition.x, currentPosition.y, 150, 18), "Physic Material:");
        settings.PhysicMaterial = EditorGUI.ObjectField(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), settings.PhysicMaterial, typeof(PhysicMaterial), false) as PhysicMaterial;
		DrawHelpSymbol(currentPosition.x, currentPosition.y, "Specifies the physic material used for the root object and all child objects.\nIf you want to use a per child physic material you should create a custom LevelPropertyApplicator.");
		
		currentPosition.y += 30;
		
		GUI.Box(new Rect(currentPosition.x - 10, currentPosition.y, currentBox.width - currentPosition.x, 100), 
		        "Isolated mesh parts are parts from the created meshes who are not connected to the rest of the created shard meshes, activating this option will create a separate mesh for each isolated mesh part. As an example, imagine two not connected spheres inside of one single mesh. This option will split both spheres and will make them independent from each other. This will increase the processing duration.");
		currentPosition.y += 80;
		settings.SplitMeshIslands = GUI.Toggle(new Rect(currentPosition.x, currentPosition.y, 400, 18), settings.SplitMeshIslands, "Search and Split Isolated Meshparts");
		
		currentPosition.y += 30;
		GUI.Box(new Rect(currentPosition.x - 10, currentPosition.y, currentBox.width - currentPosition.x, 70), 
		        "Simplified collision meshes are used as MeshColliders meshes. They contain a maximum of 255 triangles and are usable for MeshCollider vs. MeshCollider collisions. This will increase the processing duration.");
		currentPosition.y += 50;
		settings.CreateCollisionMeshs = GUI.Toggle(new Rect(currentPosition.x, currentPosition.y, 400, 18), settings.CreateCollisionMeshs, "Create Simplified Collision Meshes");
		
		return currentBox;
	}
	
	Rect DrawProcessingGUI(Rect currentBox, bool estimateSizeOnly)
	{
		currentBox.height = 100;
		if (estimateSizeOnly)
			return currentBox;
		var currentPosition = currentBox;
		currentPosition.x += 20;
		currentPosition.y += 50;
		
		GUI.Box(currentBox, "Select the name for the head prefab that Piecemaker will create. This prefab could be placed anywhere in your scene. Click 'Process' to start the processing which will slice your selected mesh and apply every setting you made in Piecemaker to the resulting shard meshes. This can take a while, so please be patient.");
		
		GUI.Label(new Rect(currentPosition.x, currentPosition.y, 150, 18), "Prefab Name:");
		settings.PrefabName = EditorGUI.TextField(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), settings.PrefabName);
		DrawHelpSymbol(currentPosition.x, currentPosition.y, string.Format("The name of the prefab Piecemaker will create. You can use this prefab in your scene.\n\nThe Path the Prefab will be stored is: {0}/{1}", Piecemaker.PiecemakerCore.plainPrefabPath, settings.PrefabName));

		currentPosition.y += 25;
		
		if (GUI.Button(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), "Process"))
		{
			core = new Piecemaker.PiecemakerCore(settings, s => CleanDirectory(s));
            if (!core.CreateInBackground())
            {
                core.Dispose();
                core = null;
            }
		}
		DrawHelpSymbol(currentPosition.x, currentPosition.y, string.Format("The 'Process' button will start the Piecemaker core process which will destruct your mesh by the settings you made.\nThis could take a while, so be patient. if you see the mouse cursor flickering while processing, this is normal Unity behaviour when doing something with Assets."));

        currentPosition = currentBox;
        currentPosition.x += 20 + 300;
        currentPosition.y += 50;

        if (GUI.Button(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), "Load Preset"))
        {
            var path = EditorUtility.OpenFilePanel("Load Piecemaker preset", lastUsedPath, "piece");
            if (!string.IsNullOrEmpty(path))
                LoadSettings(path);
            this.Repaint();
        }
        DrawHelpSymbol(currentPosition.x, currentPosition.y, string.Format("Loads piecemaker settings from a file. This allow you to load common settings when needed without much work."));

        currentPosition.y += 25;

        if (GUI.Button(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), "Save Preset"))
        {
            var path = EditorUtility.SaveFilePanel("Save Piecemaker preset", lastUsedPath, settings.PrefabName + ".piece", "piece");
            if (!string.IsNullOrEmpty(path))
                SaveSettings(path);
        }
        DrawHelpSymbol(currentPosition.x, currentPosition.y, string.Format("Stores the current settings into a file. This allow you to store common settings and load it when needed."));

		return currentBox;
	}

    protected void GenerateLastUsedPath(string path)
    {
        var lastDelimiter = path.LastIndexOfAny(new char[] { '/', '\\' });
        if (lastDelimiter < 0)
            return;
        lastUsedPath = path.Remove(lastDelimiter);
    }

    protected void LoadSettings(string path)
    {
        GenerateLastUsedPath(path);

        if (!File.Exists(path))
        {
            EnsureSettingsSerialization();
            return;
        }

        using (var streamReader = File.OpenText(path))
        {
            var oldSettings = settings;
            try
            {
                settings = SerializaahNS.Serializers.Serializer.Deserialize<Piecemaker.Settings>(streamReader.ReadToEnd());
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogWarning("Unable to load preset. " + e.ToString());
                settings = oldSettings;
            }
        }

        EnsureSettingsSerialization();

        this.Repaint();
    }

    private void EnsureSettingsSerialization()
    {
        if (settings == null)
            return;
        UnityEditor.EditorApplication.playmodeStateChanged = () =>
        {
            var isPlaying = UnityEditor.EditorApplication.isPlaying;
            var isChangingOrPlaying = UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;

            if ((!isPlaying && isChangingOrPlaying) || (isPlaying && !isChangingOrPlaying))
                settings.SaveChanges();
            else if ((isPlaying && isChangingOrPlaying) || (!isPlaying && !isChangingOrPlaying))
                settings.Load(true);
        };
    }
	
	Rect DrawCutAreaGUI(Rect currentBox, bool estimateSizeOnly)
	{
		currentBox.height = 140 + 
			EstimateHeightForProperties(settings.SliceAreaSettings.UVMapper, true, true) + 
			EstimateHeightForProperties(settings.SliceAreaSettings.Triangulator, true, true);
		if (estimateSizeOnly)
			return currentBox;
		
		var currentPosition = currentBox;
		currentPosition.x += 20;
		currentPosition.y += 70;
		
		GUI.Box(currentBox, "Select how your slice areas should look like. Select the material for the cut, how texture coordinates for the new cut area should be generated and how cut areas triangles should be generated (select 'Delaunay' if you encounter problems).");
		
		GUI.Label(new Rect(currentPosition.x, currentPosition.y, 150, 18), "Cut Material:");
        settings.CutMaterial = EditorGUI.ObjectField(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), settings.CutMaterial, typeof(Material), false) as Material;
		DrawHelpSymbol(currentPosition.x, currentPosition.y, string.Format("The material which will be used for the areas in which a slice appeared. If you slice a melon, the cut area would be the reddish inner part of the melon.\nIf you dont select any material the material you sepecified for the mesh at the first submesh index will be used."));
		
		currentPosition.y += 20;
		GUI.Label(new Rect(currentPosition.x, currentPosition.y, 150, 18), "UV Type:");
		var currentMapperIndex = System.Array.FindIndex(UVMapperAttribute.GetAvailable(), a => a.Value == settings.SliceAreaSettings.UVMapper.GetType());
		var newMapperIndex = EditorGUI.Popup(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), currentMapperIndex, UVMapperAttribute.GetAvailable().Select(a => a.Key.Name).ToArray());
		if (newMapperIndex != currentMapperIndex)
			settings.SliceAreaSettings.UVMapper = (UVMapper)System.Activator.CreateInstance(UVMapperAttribute.GetAvailable()[newMapperIndex].Value);
		DrawHelpSymbol(currentPosition.x, currentPosition.y, string.Format("How should Piecemaker calculate the texture coordinates for the cut area."));
		
		currentPosition.y = DrawPropertiesGUI(settings.SliceAreaSettings.UVMapper, true, true, (int)currentPosition.x, (int)currentPosition.y);
		
		currentPosition.y += 20;
		GUI.Label(new Rect(currentPosition.x, currentPosition.y, 150, 18), "Triangulation Type:");
		currentMapperIndex = System.Array.FindIndex(TriangulatorAttribute.GetAvailable(), a => a.Value == settings.SliceAreaSettings.Triangulator.GetType());
		newMapperIndex = EditorGUI.Popup(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), currentMapperIndex, TriangulatorAttribute.GetAvailable().Select(a => a.Key.Name).ToArray());
		if (newMapperIndex != currentMapperIndex)
			settings.SliceAreaSettings.Triangulator = (Triangulator)System.Activator.CreateInstance(TriangulatorAttribute.GetAvailable()[newMapperIndex].Value);
		DrawHelpSymbol(currentPosition.x, currentPosition.y, string.Format("How should Piecemaker calculate the triangles for the cut area. in most cases the 'Ear clipping' type is the best type, if you encounter strange or unexpected results, you can try the 'Delaunay' type which is not so efficient but will also get the job done.\n\nEar Clipping: Good for convex and concave areas.\nDelaunay: Good for convex areas. Fast."));
		
		currentPosition.y = DrawPropertiesGUI(settings.SliceAreaSettings.Triangulator, true, true, (int)currentPosition.x, (int)currentPosition.y);
		
		return currentBox;
	}
	
	Rect DrawDestructionLevelOverview(Rect currentBox, bool estimateSizeOnly)
	{
		if (estimateSizeOnly)
			return currentBox;
		var currentPosition = currentBox;
		currentPosition.x += 20;
		currentPosition.y += 60;
        
        GUI.Box(currentBox, "Select the properties of each destruction level. Each level will be spawned as prefab if the parent destructable is destroyed. Keep the destruction level count to a minimum to ensure fast processing time and a lower performance impact while in play mode.");
		
        GUI.Label(new Rect(currentPosition.x, currentPosition.y, 150, 18), "Level Count:");
        var newSize = EditorGUI.IntField(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), settings.LevelData.Count - 1);
        DrawHelpSymbol(currentPosition.x, currentPosition.y, string.Format("How often could your mesh be destructed. Each level represents the shards created when destructing a parent shard. For example, a level count of 2 means that you could destruct your base mesh, after this has been done you are able to destruct the shards of this destruction one time more. You should leave the count as low as possible to decrease the processing time. A too high number will block your computer while processing for a very long time and may produce errors when the shard size will get too low."));
        
        if (newSize < 1)
            newSize = 1;
		
        if (newSize != settings.LevelData.Count - 1)
        {
            if (newSize > settings.LevelData.Count - 1)
            {
                while (newSize > settings.LevelData.Count - 1)
                {
                    var newLevel = new Piecemaker.LevelData();
                    settings.LevelData.Add(newLevel);
                }
            }
            else
            {
                settings.LevelData = settings.LevelData.Take(newSize + 1).ToList();
            }
        }
        
        currentPosition.y += 30;
		
        GUI.Box(new Rect(currentBox.x, currentPosition.y, currentBox.width, 1), string.Empty);
		
        var levelPropertiesHeight = -1;
        for(int i = 0; i < settings.LevelData.Count; ++i)
        {		
            var levelData = settings.LevelData[i];
            levelPropertiesHeight += 95;
			
            if (!levelData.IsRoot)
                levelPropertiesHeight += 100;
			
            foreach (var levelPropertyTemplatePair in Piecemaker.LevelPropertyApplicatorAttribute.GetAvailable())
            {
                var property = levelData.PropertyApplicators.FirstOrDefault(p => p.GetType() == levelPropertyTemplatePair.Value);
                var hasProperty = property != null;
                
                if (hasProperty)
                    levelPropertiesHeight += EstimateHeightForProperties(property, levelData.IsRoot, i + 1 == settings.LevelData.Count);
            }
        }
		
        levelOverviewScrollPos = GUI.BeginScrollView(
            new Rect(currentBox.x, currentPosition.y + 1, currentBox.width - 1, currentBox.height - currentPosition.y + 3),
            levelOverviewScrollPos,
            new Rect(currentBox.x, currentPosition.y + 1, currentBox.width - 15, levelPropertiesHeight));
		
        for(int i = 0; i < settings.LevelData.Count; ++i)
        {		
            var levelData = settings.LevelData[i];
            string name = levelData.IsRoot ? "Root" : "Level " + i.ToString();
			
            EditorGUI.DropShadowLabel(new Rect(currentBox.x, currentPosition.y, currentBox.width, 18), name);
            currentPosition.y += 30;
			
            if (!levelData.IsRoot)
            {
                GUI.Label(new Rect(currentPosition.x, currentPosition.y, 150, 18), "Slice Count On X Axis:");
                levelData.SliceXCount = EditorGUI.IntField(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), levelData.SliceXCount);
                DrawHelpSymbol(currentPosition.x, currentPosition.y, string.Format("How often should the mesh sliced along the X axis. A higher value will increase processing time and the number of generated shards."));
                currentPosition.y += 20;
				
                GUI.Label(new Rect(currentPosition.x, currentPosition.y, 150, 18), "Slice Count On Y Axis:");
                levelData.SliceYCount = EditorGUI.IntField(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), levelData.SliceYCount);
                DrawHelpSymbol(currentPosition.x, currentPosition.y, string.Format("How often should the mesh sliced along the Y axis. A higher value will increase processing time and the number of generated shards."));
                currentPosition.y += 20;
				
                GUI.Label(new Rect(currentPosition.x, currentPosition.y, 150, 18), "Slice Count On Z Axis:");
                levelData.SliceZCount = EditorGUI.IntField(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), levelData.SliceZCount);
                DrawHelpSymbol(currentPosition.x, currentPosition.y, string.Format("How often should the mesh sliced along the X axis. A higher value will increase processing time and the number of generated shards."));
                currentPosition.y += 20;
				
                levelData.SliceChaos = EditorGUI.Vector3Field(new Rect(currentPosition.x, currentPosition.y, 350, 18), "Slice Plane Random Rotation Degree Maximum:", levelData.SliceChaos);
                DrawHelpSymbol(currentPosition.x, currentPosition.y, string.Format("The maximum angle in degree which the slice planes will be random rotated. Increasing this values will ensure a more natural look for each shard."));
                currentPosition.y += 40;
            }
			
            GUI.Label(new Rect(currentPosition.x, currentPosition.y, 150, 18), "Prefab Template:");
            levelData.PrefabTemplate = EditorGUI.ObjectField(new Rect(currentPosition.x + 150, currentPosition.y, 200, 18), levelData.PrefabTemplate, typeof(GameObject), false) as GameObject;
            DrawHelpSymbol(currentPosition.x, currentPosition.y, string.Format("The prefab which will be used to create the shards, if nothing is given a default GameObject will be used. Use this if you want to put special behaviour on each of your shards without using LevelPropertyApplicators (a class which is designed to perform custom script logic for each shard after the shard has been created.)."));
            currentPosition.y += 40;
			
            foreach (var levelPropertyTemplatePair in Piecemaker.LevelPropertyApplicatorAttribute.GetAvailable())
            {
                var property = levelData.PropertyApplicators.FirstOrDefault(p => p.GetType() == levelPropertyTemplatePair.Value);
                var hasProperty = property != null;

                var propertyName = ObjectNames.NicifyVariableName(levelPropertyTemplatePair.Key.Name);
				
                var wantHaveProperty = GUI.Toggle(new Rect(currentPosition.x, currentPosition.y, 400, 18), hasProperty, propertyName);
                currentPosition.y += 20;
                if (wantHaveProperty != hasProperty)
                {
                    if (!wantHaveProperty)
                    {
                        levelData.PropertyApplicators.Remove(property);
                        property = null;
                        hasProperty = false;
                    }
                    else
                    {
                        property = (Piecemaker.LevelPropertyApplicator)System.Activator.CreateInstance(levelPropertyTemplatePair.Value);
                        levelData.PropertyApplicators.Add(property);
                        hasProperty = true;
                    }
                }
				
                if (hasProperty)
                    currentPosition.y = DrawPropertiesGUI(property, levelData.IsRoot, i + 1 == settings.LevelData.Count, (int)currentPosition.x, (int)currentPosition.y);
            }
			
			
            currentPosition.y += 5;
            GUI.Box(new Rect(currentBox.x, currentPosition.y, currentBox.width, 1), string.Empty);
        }
		
        GUI.EndScrollView();
        
		return currentBox;
	}
	
	int EstimateHeightForProperties(object target, bool isRootObject, bool isLastObject)
	{
		if (target == null)
			return 0;
		
		var height = 0;
		var firstProperty = true;
		var properties = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToArray();
		foreach (var property in properties)
		{
			if (property.DeclaringType == typeof(UnityEngine.Object))
				continue;
			if (property.GetCustomAttributes(typeof(HideInInspector), false).Length != 0)
				continue;
			if (isRootObject && property.GetCustomAttributes(typeof(Piecemaker.ApplyOnlyOnChildsAttribute), false).Length != 0)
				continue;
            if (!isLastObject && property.GetCustomAttributes(typeof(Piecemaker.ApplyOnlyOnLastChildAttribute), false).Length != 0)
                continue;
			
			if (firstProperty)
			{
				height += 20;
				firstProperty = false;
			}
			
			if (property.PropertyType == typeof(Vector2))
				height += 14;
			else if (property.PropertyType == typeof(Vector3))
				height += 14;
			else if (property.PropertyType == typeof(Vector4))
				height += 14;
			
			height += 20;
		}
		
		return height;
	}
	
	int DrawPropertiesGUI(object target, bool isRootObject, bool isLastObject, int posX, int posY)
	{
		if (target == null)
			return posY;
		
		var firstProperty = true;
		var properties = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToArray();
		foreach (var property in properties)
		{
			if (property.DeclaringType == typeof(UnityEngine.Object))
				continue;
			if (property.GetCustomAttributes(typeof(HideInInspector), false).Length != 0)
				continue;
			if (isRootObject && property.GetCustomAttributes(typeof(Piecemaker.ApplyOnlyOnChildsAttribute), false).Length != 0)
				continue;
            if (!isLastObject && property.GetCustomAttributes(typeof(Piecemaker.ApplyOnlyOnLastChildAttribute), false).Length != 0)
				continue;
			
			if (firstProperty)
			{
				posY += 20;
				firstProperty = false;
			}
            var name = ObjectNames.NicifyVariableName(property.Name) + ":";
			var rawValue = property.GetValue(target, null);
			if (property.PropertyType == typeof(float))
			{
				GUI.Label(new Rect(posX, posY, 150, 18), name);
				property.SetValue(target, (float)EditorGUI.FloatField(new Rect(posX + 150, posY, 200, 18), (float)rawValue), null);
			}
			else if (property.PropertyType == typeof(int))
			{
				GUI.Label(new Rect(posX, posY, 150, 18), name);
				if (property.Name.ToLower().Contains("layer"))
					property.SetValue(target, EditorGUI.LayerField(new Rect(posX + 150, posY, 200, 18), (int)rawValue), null);
				else
					property.SetValue(target, EditorGUI.IntField(new Rect(posX + 150, posY, 200, 18), (int)rawValue), null);
			}
			else if (property.PropertyType == typeof(bool))
			{
				GUI.Label(new Rect(posX, posY, 150, 18), name);
                property.SetValue(target, EditorGUI.Popup(new Rect(posX + 150, posY, 200, 18), ((bool)rawValue) ? 1 : 0, new string[] { "False", "True" }) == 0 ? false : true, null);
			}
			else if (property.PropertyType == typeof(string))
			{
				GUI.Label(new Rect(posX, posY, 150, 18), name);
				property.SetValue(target, EditorGUI.TextField(new Rect(posX + 150, posY, 200, 18), (string)rawValue), null);
			}
			else if (property.PropertyType == typeof(Vector2))
			{
				property.SetValue(target, EditorGUI.Vector2Field(new Rect(posX + 150, posY, 200, 18), name, (Vector2)rawValue), null);
				posY += 14;
			}
			else if (property.PropertyType == typeof(Vector3))
			{
				property.SetValue(target, EditorGUI.Vector3Field(new Rect(posX + 150, posY, 200, 18), name, (Vector3)rawValue), null);
				posY += 14;
			}
			else if (property.PropertyType == typeof(Vector4))
			{
				property.SetValue(target, EditorGUI.Vector4Field(new Rect(posX + 150, posY, 200, 18), name, (Vector4)rawValue), null);
				posY += 14;
			}
            else if (property.PropertyType == typeof(GameObject))
            {
                GUI.Label(new Rect(posX, posY, 150, 18), name);
                property.SetValue(target, EditorGUI.ObjectField(new Rect(posX + 150, posY, 200, 18), (Object)rawValue, property.PropertyType, false), null);
            }
			else 
			{
				GUI.Label(new Rect(posX, posY, 150, 18), name);
				if (rawValue is System.Enum)
				{
					property.SetValue(target, EditorGUI.EnumPopup(new Rect(posX + 150, posY, 200, 18), (System.Enum)rawValue), null);
				}
			}
			
			var helpAttribute = (HelpAttribute)property.GetCustomAttributes(typeof(HelpAttribute), false).FirstOrDefault();
			if (helpAttribute != null)
				DrawHelpSymbol(posX, posY, helpAttribute.Text);
			
			posY += 20;
		}
		
		return posY;
	}
	
	void DrawHelpSymbol(float posX, float posY, string helpText)
	{
		if (GUI.Button(new Rect(posX + 360, posY, 18, 15), "?"))
			EditorUtility.DisplayDialog("Help", helpText, "Ok");
	}
	
	protected abstract void CleanDirectory(string path);
    protected abstract void SaveSettings(string path);
}
