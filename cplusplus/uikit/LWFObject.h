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

#import <Foundation/Foundation.h>

@class LWFView;

typedef void (^EventHandler)();

/**
 `LWFObject` is a kind of Adobe Flash Player and a representative of a C++ LWF
 instance. It loads LWF data, which is converted from Adobe Flash, and plays as
 Adobe Flash Player. You can controll the animation in the LWF data via various
 APIs like Flash ActionScript 1.0.
 */
@interface LWFObject : NSObject

/**
 The C++ LWF instance. You can directly manipulate it using C++ LWF APIs.
 */
@property (readonly) void *lwf;

/**
 The name of the loaded LWF data.
 */
@property (readonly) NSString *name;

/**
 The color of the stage background in Adobe Flash.
 */
@property (readonly) unsigned int backgroundColor;

/**
 If the value of this paramter is `NO`, then user cannot interact the LWF data
 by touch screen.
 */
@property (nonatomic, assign) BOOL interactive;

/**
 The frameRate of the Adobe Flash data. You can change the speed of animation
 via this paramter.
 */
@property (nonatomic, assign) NSInteger frameRate;

/**
 The width of the stage in Adobe Flash.
 */
@property (readonly) float width;

/**
 The height of the stage in Adobe Flash.
 */
@property (readonly) float height;

/**
 Creates and returns a `LWFObject` object with specified the LWF data.

 @param path The specified path of the LWF data in the main bundle,
 like "sample.lwf".
 @param view The specified `LWFView` object.
 */
+ (id)lwfWithFile:(NSString *)path view:(LWFView *)view;

/**
 Creates and returns a `LWFObject` object with specified the LWF data.

 @param path The specified path of the LWF data in the main bundle,
 like "sample.lwf".
 @param view The specified `LWFView` object.
 */
- (id)initWithFile:(NSString *)path view:(LWFView *)view;

/**
 Initializes the C++ LWF instance. It means starting over the animation at all.
 */
- (void)lwfInit;

/**
 Fits the height of the Flash stage size to the height of the `LWFView` frame
 size.
 */
- (void)fitForHeight:(CGSize)size;

/**
 Fits the width of the Flash stage size to the width of the `LWFView` frame
 size.
 */
- (void)fitForWidth:(CGSize)size;

/**
 Updates the animation. This method is automatically called from `LWFView`.
 */
- (void)updateWithTick:(float)tick;

/**
 Draws the animation. This method is automatically called from `LWFView`.
 */
- (void)draw;

/**
 Adds a handler for an event, which will be fired from
 fscommand("event", "eventName") in the animation.

 @param eventName The specified event name by fscommand("event", "eventName")
 in the Adobe Flash data.
 @param handler A block object to be executed when the event is fired.

 @return An Id of the event handler.
 */
- (int)addEventHandler:(NSString *)eventName handler:(EventHandler)handler;

/**
 Removes the event handler.

 @param eventName The specified event name by fscommand("event", "eventName")
 in the Adobe Flash data.
 @param handlerId The Id of the event handler.
 */
- (void)removeEventHandler:(NSString *)eventName handlerId:(int)handlerId;

/**
 Clears the all event handlers for the specified event.

 @param eventName The specified event name by fscommand("event", "eventName")
 in the Adobe Flash data.
 */
- (void)clearEventHandler:(NSString *)eventName;

/**
 Clears the all event handlers in the LWF instance.
 */
- (void)clearAllEventHandlers;

/**
 Inputs a touch position to the LWF instance. This method is automatically
 called from `LWFView`.

 @param point The position of a touch event.
 */
- (BOOL)inputPoint:(CGPoint)point;

/**
 Inputs a touch press event to the LWF instance. This method is automatically
 called from `LWFView`.
 */
- (void)inputPress;

/**
 Inputs a touch release event to the LWF instance. This method is automatically
 called from `LWFView`.
 */
- (void)inputRelease;

/**
 Starts the animation of the target movie clip as Adobe Flash ActionScript.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 */
- (void)play:(NSString *)target;

/**
 Stops the animation of the target movie clip as Adobe Flash ActionScript.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 */
- (void)stop:(NSString *)target;

/**
 Moves forward the animation of the target movie clip as Adobe Flash
 ActionScript.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 */
- (void)nextFrame:(NSString *)target;

/**
 Moves backword the animation of the target movie clip as Adobe Flash
 ActionScript.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 */
- (void)prevFrame:(NSString *)target;

/**
 Changes the visibility of the target movie clip as Adobe Flash ActionScript.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 */
- (void)setVisible:(NSString *)target visible:(BOOL)visible;

/**
 Moves the frame in the animation of the target movie clip as Adobe Flash
 ActionScript. It also calls `stop` method.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 @param frame The frame number, 1-origin.
 */
- (void)gotoAndStop:(NSString *)target frame:(int)frame;

/**
 Moves the frame in the animation of the target movie clip as Adobe Flash
 ActionScript. It also calls `stop` method.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 @param frame The frame number, 1-origin.
 */
- (void)gotoAndStop:(NSString *)target label:(NSString *)label;

/**
 Moves the frame in the animation of the target movie clip as Adobe Flash
 ActionScript. It also calls `play` method.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 @param frame The frame number, 1-origin.
 */
- (void)gotoAndPlay:(NSString *)target frame:(int)frame;

/**
 Moves the frame in the animation of the target movie clip as Adobe Flash
 ActionScript. It also calls `play` method.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 @param frame The frame number, 1-origin.
 */
- (void)gotoAndPlay:(NSString *)target label:(NSString *)label;

/**
 Moves the position of the target movie clip relative to its current position
 in coordinates of the parent movie clip as Adobe Flash ActionScript.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 @param x An integer in the x coordinate of the parent movie clip.
 @param y An integer in the y coordinate of the parent movie clip.
 */
- (void)move:(NSString *)target x:(float)x y:(float)y;

/**
 Moves the position of the target movie clip in coordinates of the parent movie
 clip as Adobe Flash ActionScript.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 @param x An integer in the x coordinate of the parent movie clip.
 @param y An integer in the y coordinate of the parent movie clip.
 */
- (void)moveTo:(NSString *)target x:(float)x y:(float)y;

/**
 Rotates the target movie clip relative to its current rotation as Adobe Flash
 ActionScript.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 @param degree The rotation of the movie clip, in degrees, from its current
 rotation.
 */
- (void)rotate:(NSString *)target degree:(float)degree;

/**
 Rotates the target movie clip as Adobe Flash ActionScript.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 @param degree The rotation of the movie clip, in degrees, from its original
 rotation.
 */
- (void)rotateTo:(NSString *)target degree:(float)degree;

/**
 Scales the size of the target movie clip relative to its current scale as
 Adobe Flash ActionScript.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 @param x The x scale of the movie clip from its current scale.
 @param y The y scale of the movie clip from its current scale.
 */
- (void)scale:(NSString *)target x:(float)x y:(float)y;

/**
 Scales the size of the target movie clip as Adobe Flash ActionScript.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 @param x The x scale of the movie clip from its original scale.
 @param y The y scale of the movie clip from its original scale.
 */
- (void)scaleTo:(NSString *)target x:(float)x y:(float)y;

/**
 Sets the transparency of the target movie clip as Adobe Flash ActionScript.

 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 @param alpha The value of transparency, in 0(transparent) to 1(opaque).
 */
- (void)setAlpha:(NSString *)target alpha:(float)alpha;

- (void)setColorTransform:(NSString *)target red:(float)red green:(float)green
	blue:(float)blue alpha:(float)alpha;

/**
 Attaches a movie into a movie clip as Adobe Flash ActionScript.

 @param linkageName The linkage name of the movie clip symbol in the library of
 the Adobe Flash data. This is the name that you enter in the Identifier field
 in the Linkage Properties dialog box.
 @param target The instance name of the target movie, like "playerMovie" or
 "_root.stageMovie.playerMovie" as Adobe Flash ActionScript.
 @param attachName A unique instance name for the movie clip being attached to
 the movie clip.

 @return `YES` is the new movie was successfully attached, otherwise `NO`.
 */
- (BOOL)attachMovie:(NSString *)linkageName
	target:(NSString *)target attachName:(NSString *)attachName;

@end
