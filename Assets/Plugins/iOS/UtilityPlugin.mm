//
//  UtilityPlugin.m
//  Unity-iPhone
//
//  Created by Nathan Swenson on 3/18/12.
//  Copyright (c) 2012 University of Utah. All rights reserved.
//

#import "UtilityPlugin.h"

@implementation UtilityPlugin

+ (void) printThing:(id)thing
{
    UIPrintInteractionController* printerController = [UIPrintInteractionController sharedPrintController];
    [printerController setPrintingItem:thing];
    [printerController presentAnimated:YES completionHandler:nil];
}

@end

// Interface that is exposed to Unity
extern "C"
{
    void _PrintARCard()
    {
        NSURL* pdf = [[NSBundle mainBundle] URLForResource:@"target_stones_USLetter" withExtension:@"pdf"];
        [UtilityPlugin printThing:pdf];
    }
    
    const char * _GetDeviceName()
    {
        const char * name = [[[UIDevice currentDevice] name] UTF8String];
        // Need returned string to be allocated on the heap as per Unity's documentation.
        char * returnName = (char *)malloc((strlen(name) + 1));
        strcpy(returnName, name);
        return returnName;
    }
    
    int _ContentScaleFactor()
    {
        float contentScaleFactor = 1.0f;
        if([[UIScreen mainScreen] respondsToSelector:@selector(scale)]) contentScaleFactor = [[UIScreen mainScreen] scale];
        return (int)contentScaleFactor;
    }
}