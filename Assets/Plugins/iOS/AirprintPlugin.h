//
//  AirprintPlugin.h
//  Unity-iPhone
//
//  Created by Nathan Swenson on 1/25/12.
//  Copyright (c) 2012 University of Utah. All rights reserved.
//

#import <Foundation/Foundation.h>

// A class encapsulating the Airprint functionality for use with Unity. See the .mm file
// for the interface that is exposed to Unity
@interface AirprintPlugin : NSObject <UIPrintInteractionControllerDelegate>

// Objective-C method to display the Airprint dialog for printing the given item. The item
// can be an NSURL, NSData, UIImage, or ALAsset. See UIPrintInteractionController's printingItem
// property for details about what can be printed. 
+ (void) printThing:(id)thing;

@end

