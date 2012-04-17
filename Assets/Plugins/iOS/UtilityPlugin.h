//
//  UtilityPlugin.h
//  Unity-iPhone
//
//  Created by Nathan Swenson on 3/18/12.
//  Copyright (c) 2012 University of Utah. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface UtilityPlugin : NSObject

// Objective-C method to display the Airprint dialog for printing the given item. The item
// can be an NSURL, NSData, UIImage, or ALAsset. See UIPrintInteractionController's printingItem
// property for details about what can be printed. 
+ (void) printThing:(id)thing;

@end
