/*
 * Copyright (C) 2013 GREE, Inc.
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

#import <UIKit/UIKit.h>

@class LWFObject;

/**
 You use the `LWFView` class to embed animation content,
 which is converted from Adobe Flash, in your application.
 */
@interface LWFView : UIView

/**
 Specify the LWF data path (like "sample.lwf"), which is converted from Adobe
 Flash. The property only works via Storyboard. The LWF data will be
 automatically added via `LWFObject` class and `addLWFObject` method.
 */
@property (nonatomic, strong) NSString *path;

/**
 The `LWFView` automatically scales animation contents and moves the center of
 it by using both the stage size of Adobe Flash data and the `LWFView` frame
 size. "fitForHeight" means fitting the height of the Flash stage size to the
 height of the `LWFView` frame size, and "fitForWidth" means the width as well.
 */
@property (nonatomic, strong) NSString *fit;

/**
 The `LWFView` uses CADisplayLink internally. The value of this parameter is
 passed through the frameInterval property of the CADisplayLink object. The
 default valus is 1.
 */
@property (nonatomic, assign) NSInteger frameInterval;

/**
 Specify `YES` the parameter if you want to fill the background of the
 `LWFView`. It uses the color of the stage in the Adobe Flash data.
 */
@property (nonatomic, assign) BOOL useBackgroundColor;

/**
 A list of `LWFObject` objects. `LWFObject` object is added by
 `path` property via Storyboard or `addLWFObject` method.
 */
@property (nonatomic, readonly) NSArray *lwfList;

/**
 Initialized a `LWFView` object with the specified frame size.
 */
- (id)initWithFrame:(CGRect)frame;

/**
 Invalidate the `LWFView` object. It invalidates internal CADisplayLink object
 and releases `lwfList`.
 */
- (void)invalidate;

/**
 Call `LWFObject:lwfInit` method of every `LWFObject` object in the `lwfList`.
 */
- (void)lwfInit;

/**
 Add `LWFObject` object into `lwfList`.
 */
- (void)addLWFObject:(LWFObject *)lwfObject;

/**
 Returns the last `LWFObject` object in the `lwfList`.
 */
- (LWFObject *)lastLWFObject;

@end
