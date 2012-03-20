//
//  AirprintPlugin.m
//  Unity-iPhone
//
//  Created by Nathan Swenson on 1/25/12.
//  Copyright (c) 2012 University of Utah. All rights reserved.
//

#import "AirprintPlugin.h"

@implementation AirprintPlugin

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
        [AirprintPlugin printThing:pdf];
    }
}