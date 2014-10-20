/*
 * Copyright (C) 2014 GREE, Inc.
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

typedef void (^EventHandler)();

@interface LWFObject : NSObject

@property (readonly) void *lwf;
@property (readonly) NSString *name;
@property (readonly) unsigned int backgroundColor;
@property (nonatomic, assign) BOOL interactive;
@property (nonatomic, assign) NSInteger frameRate;
@property (readonly) float width;
@property (readonly) float height;

+ (id)lwfWithFile:(NSString *)path context:(EAGLContext *)context;
- (id)initWithFile:(NSString *)path context:(EAGLContext *)context;

- (void)lwfInit;
- (void)fitForHeight:(CGSize)size;
- (void)fitForWidth:(CGSize)size;

- (void)updateWithTick:(float)tick;
- (void)draw;

- (int)addEventHandler:(NSString *)eventName handler:(EventHandler)handler;
- (void)removeEventHandler:(NSString *)eventName handlerId:(int)handlerId;
- (void)clearEventHandler:(NSString *)eventName;
- (void)clearAllEventHandlers;

- (BOOL)inputPoint:(CGPoint)point;
- (void)inputPress;
- (void)inputRelease;

- (void)play:(NSString *)target;
- (void)stop:(NSString *)target;
- (void)nextFrame:(NSString *)target;
- (void)prevFrame:(NSString *)target;
- (void)setVisible:(NSString *)target visible:(BOOL)visible;
- (void)gotoAndStop:(NSString *)target frame:(int)frame;
- (void)gotoAndStop:(NSString *)target label:(NSString *)label;
- (void)gotoAndPlay:(NSString *)target frame:(int)frame;
- (void)gotoAndPlay:(NSString *)target label:(NSString *)label;
- (void)move:(NSString *)target x:(float)x y:(float)y;
- (void)moveTo:(NSString *)target x:(float)x y:(float)y;
- (void)rotate:(NSString *)target degree:(float)degree;
- (void)rotateTo:(NSString *)target degree:(float)degree;
- (void)scale:(NSString *)target x:(float)x y:(float)y;
- (void)scaleTo:(NSString *)target x:(float)x y:(float)y;
- (void)setAlpha:(NSString *)target alpha:(float)alpha;
- (void)setColorTransform:(NSString *)target red:(float)red green:(float)green
	blue:(float)blue alpha:(float)alpha;

- (void)attachMovie:(NSString *)linkageName
	target:(NSString *)target attachName:(NSString *)attachName;
- (void)attachLWF:(LWFObject *)lwfObject
	target:(NSString *)target attachName:(NSString *)attachName;

- (void)setText:(NSString *)textName text:(NSString *)text;
- (NSString *)getText:(NSString *)textName;

@end
