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

#import "LWFView.h"
#import "LWFObject.h"

@interface LWFView ()
@property (nonatomic, strong) CADisplayLink *displayLink;
@property (nonatomic, strong) NSMutableArray *displayList;
@end

@implementation LWFView

- (void)awakeFromNib
{
	[self start];
}

- (id)initWithFrame:(CGRect)frame
{
	self = [super initWithFrame:frame];
	[self start];
	return self;
}

- (void)start
{
	if (!self.displayLink) {
		self.displayList = [NSMutableArray array];

		self.displayLink = [CADisplayLink
			displayLinkWithTarget:self selector:@selector(update:)];
		[self.displayLink setFrameInterval:(
			self.frameInterval <= 0 ? 1 : self.frameInterval)];
		[self.displayLink addToRunLoop:[NSRunLoop currentRunLoop]
			forMode:NSRunLoopCommonModes];

		if (self.path) {
			LWFObject *lwfObject = [LWFObject lwfWithFile:self.path view:self];
			[self addLWFObject:lwfObject];
		}
	}
}

- (void)invalidate
{
	if (self.displayLink) {
		[self.displayLink invalidate];
		self.displayLink = nil;
		self.displayList = nil;
	}
}

- (void)addLWFObject:(LWFObject *)lwfObject
{
	[self.displayList addObject:lwfObject];
	if (self.displayList.count == 1 && self.useBackgroundColor) {
		unsigned int c = lwfObject.backgroundColor;
		float red = ((c >> 0) & 0xff) / 255.0f;
		float green = ((c >> 8) & 0xff) / 255.0f;
		float blue = ((c >> 16) & 0xff) / 255.0f;
		float alpha = 1.0f;
		self.backgroundColor =
			[UIColor colorWithRed:red green:green blue:blue alpha:alpha];
	}
}

- (void)update:(CADisplayLink *)sender
{
	CFTimeInterval tick = sender.duration * sender.frameInterval;

	for (LWFObject *lwfObject in self.displayList) {
		if ([self.fit caseInsensitiveCompare:@"fitForHeight"] == NSOrderedSame)
			[lwfObject fitForHeight:CGSizeMake(
				self.frame.size.width, self.frame.size.height)];
		else if ([self.fit
				caseInsensitiveCompare:@"fitForWidth"] == NSOrderedSame)
			[lwfObject fitForWidth:CGSizeMake(
				self.frame.size.width, self.frame.size.height)];
		[lwfObject updateWithTick:tick];
	}

	for (LWFObject *lwfObject in self.displayList)
		[lwfObject draw];
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
