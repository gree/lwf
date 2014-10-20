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

#import "LWFView.h"
#import "LWFObject.h"
#import "LWFShader.h"

@interface WeakReference : NSObject
{
	__weak id nonretainedObjectValue;
	__unsafe_unretained id originalObjectValue;
}

+ (WeakReference *)weakReferenceWithObject:(id)object;

- (id)nonretainedObjectValue;
- (void *)originalObjectValue;
@end

@implementation WeakReference

- (id)initWithObject:(id)object
{
	self = [super init];
	if (self)
		nonretainedObjectValue = originalObjectValue = object;
	return self;
}

+ (WeakReference *)weakReferenceWithObject:(id)object
{
	return [[self alloc] initWithObject:object];
}

- (id)nonretainedObjectValue
{
	return nonretainedObjectValue;
}

- (void *)originalObjectValue
{
	return (__bridge void *) originalObjectValue;
}

- (BOOL)isEqual:(WeakReference *)object
{
	if (![object isKindOfClass:[WeakReference class]])
		return NO;
	return object.originalObjectValue == self.originalObjectValue;
}

@end

static NSMutableArray *eaglContextArray;

@interface LWFView ()
{
	BOOL _active;
	int _counter;
	float _red;
	float _green;
	float _blue;
}

@property (nonatomic, strong) CADisplayLink *displayLink;
@property (nonatomic, strong) NSMutableArray *displayList;
@end

@implementation LWFView

- (void)awakeFromNib
{
	self.displayList = [NSMutableArray array];
	if (self.path) {
        [self glInit];
		LWFObject *lwfObject =
            [LWFObject lwfWithFile:self.path context:self.context];
		[self addLWFObject:lwfObject];
	}
}

- (id)initWithFrame:(CGRect)frame
{
	self = [super initWithFrame:frame];
	self.displayList = [NSMutableArray array];
	return self;
}

- (void)glInit
{
	static dispatch_once_t once;
	dispatch_once(&once, ^{
		eaglContextArray = [NSMutableArray array];
	});

	self.backgroundColor = [UIColor clearColor];
	self.opaque = NO;

	NSMutableIndexSet *indexes = [NSMutableIndexSet indexSet];
	EAGLContext *context = nil;
	for (NSUInteger i = 0; i < eaglContextArray.count; ++i) {
		WeakReference *ref = [eaglContextArray objectAtIndex:i];
		EAGLContext *c = [ref nonretainedObjectValue];
		if (c) {
			if (!context)
				context = c;
		} else {
			[indexes addIndex:i];
		}
	}
	[eaglContextArray removeObjectsAtIndexes:indexes];

	self.context = [[EAGLContext alloc] initWithAPI:kEAGLRenderingAPIOpenGLES2
		sharegroup:[context sharegroup]];
	[eaglContextArray
		addObject:[WeakReference weakReferenceWithObject:self.context]];

	[EAGLContext setCurrentContext:self.context];
	glEnable(GL_BLEND);
	glDisable(GL_DEPTH_TEST);
	glDisable(GL_DITHER);
	glDisable(GL_SCISSOR_TEST);
	glEnable(GL_TEXTURE_2D);
	glActiveTexture(GL_TEXTURE0);
	glEnableClientState(GL_VERTEX_ARRAY);
	glEnableClientState(GL_TEXTURE_COORD_ARRAY);
	glEnableClientState(GL_COLOR_ARRAY);
	if (!context)
		LWF::LWFShader::shared()->init();
	[EAGLContext setCurrentContext:nil];
}

- (void)didMoveToWindow
{
	if (self.window) {
		_active = YES;
		if (!self.displayLink && self.displayList) {
            if (!self.context)
                [self glInit];
			self.displayLink = [CADisplayLink
				displayLinkWithTarget:self selector:@selector(update:)];
			[self.displayLink setFrameInterval:(
				self.frameInterval <= 0 ? 1 : self.frameInterval)];
			[self.displayLink addToRunLoop:[NSRunLoop currentRunLoop]
				forMode:NSRunLoopCommonModes];
		}
	} else {
		_active = NO;
		_counter = 3;
	}
}

- (void)invalidate
{
	[self.displayLink invalidate];
	self.displayLink = nil;
	[EAGLContext setCurrentContext:nil];
	self.context = nil;
	self.displayList = nil;
}

- (void)addLWFObject:(LWFObject *)lwfObject
{
	[self.displayList addObject:lwfObject];
	if (self.displayList.count == 1 && self.useBackgroundColor) {
		unsigned int c = lwfObject.backgroundColor;
		_red = ((c >> 0) & 0xff) / 255.0f;
		_green = ((c >> 8) & 0xff) / 255.0f;
		_blue = ((c >> 16) & 0xff) / 255.0f;
	}
}

- (NSArray *)lwfList
{
	return self.displayList;
}

- (LWFObject *)lastLWFObject
{
	return [self.displayList lastObject];
}

- (void)lwfInit
{
	if (self.displayList) {
		for (LWFObject *lwfObject in self.displayList) {
			[lwfObject lwfInit];
		}
	}
}

- (void)update:(CADisplayLink *)sender
{
	if (!_active) {
		if (--_counter > 0)
			return;

		[self.displayLink invalidate];
		self.displayLink = nil;
	    [EAGLContext setCurrentContext:nil];
	    self.context = nil;
		return;
	}

	CFTimeInterval tick = sender.duration * sender.frameInterval;
	[EAGLContext setCurrentContext:self.context];

	for (LWFObject *lwfObject in self.displayList) {
		if ([self.fit caseInsensitiveCompare:@"fitForHeight"] == NSOrderedSame)
			[lwfObject fitForHeight:CGSizeMake(
				self.drawableWidth, self.drawableHeight)];
		else if ([self.fit
				caseInsensitiveCompare:@"fitForWidth"] == NSOrderedSame)
			[lwfObject fitForWidth:CGSizeMake(
				self.drawableWidth, self.drawableHeight)];
		[lwfObject updateWithTick:tick];
	}

	if (self.useBackgroundColor)
		glClearColor(_red, _green, _blue, 1);
	else
		glClearColor(0, 0, 0, 0);
	glClear(GL_COLOR_BUFFER_BIT);

	for (LWFObject *lwfObject in self.displayList)
		[lwfObject draw];

	[EAGLContext setCurrentContext:nil];

	[self display];
}

- (void)touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event
{
	UITouch *touch = [touches anyObject];
	CGPoint point = [touch locationInView:self];
	for (LWFObject *lwfObject in self.displayList) {
		if (lwfObject.interactive) {
			[lwfObject inputPoint:point];
			[lwfObject inputPress];
		}
	}
}

- (void)touchesMoved:(NSSet *)touches withEvent:(UIEvent *)event
{
	UITouch *touch = [touches anyObject];
	CGPoint point = [touch locationInView:self];
	for (LWFObject *lwfObject in self.displayList) {
		if (lwfObject.interactive)
			[lwfObject inputPoint:point];
	}
}

- (void)touchesEnded:(NSSet *)touches withEvent:(UIEvent *)event
{
	UITouch *touch = [touches anyObject];
	CGPoint point = [touch locationInView:self];
	for (LWFObject *lwfObject in self.displayList) {
		if (lwfObject.interactive) {
			[lwfObject inputPoint:point];
			[lwfObject inputRelease];
		}
	}
}

@end
