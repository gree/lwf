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

#import "LWFObject.h"
#import "LWFView.h"
#import "LWFRendererFactory.h"
#import "LWFResourceCache.h"
#import "lwf.h"

using namespace LWF;

@interface LWFObject ()
{
	shared_ptr<class LWF> lwf;
}
@end

@implementation LWFObject

+ (id)lwfWithFile:(NSString *)path view:(LWFView *)view
{
	return [[self alloc] initWithFile:path view:view];
}

- (id)initWithFile:(NSString *)path view:(LWFView *)view
{
	self = [super init];
	if (!self)
		return nil;

	string pathstr = [path UTF8String];
	shared_ptr<Data> data = LWFResourceCache::shared()->loadLWFData(pathstr);
	if (!data)
		return nil;

	shared_ptr<LWFRendererFactory> factory =
		make_shared<LWFRendererFactory>(view);

	lwf = make_shared<class LWF>(data, factory);
	if (!lwf || !factory) {
		LWFResourceCache::shared()->unloadLWFData(data);
		return nil;
	}

	return self;
}

- (void)dealloc
{
	shared_ptr<Data> data = lwf->data;
	lwf->Destroy();
	LWFResourceCache::shared()->unloadLWFData(data);
}

- (NSString *)name
{
	return [NSString stringWithUTF8String:lwf->name.c_str()];
}

- (unsigned int)backgroundColor
{
	return lwf->data->header.backgroundColor;
}

- (BOOL)interactive
{
	return lwf->interactive;
}

- (void)setInteractive:(BOOL)i
{
	lwf->interactive = i;
}

- (NSInteger)frameRate
{
	return lwf->frameRate;
}

- (void)setFrameRate:(NSInteger)f
{
	lwf->SetFrameRate(f);
}

- (float)width
{
	return lwf->width;
}

- (float)height
{
	return lwf->height;
}

- (void)lwfInit
{
	lwf->Init();
}

- (void)fitForHeight:(CGSize)size
{
	lwf->FitForHeight(size.width, size.height);
}

- (void)fitForWidth:(CGSize)size
{
	lwf->FitForWidth(size.width, size.height);
}

- (void)updateWithTick:(float)tick
{
	lwf->Exec(tick);
}

- (void)draw
{
	lwf->Render();
}

- (int)addEventHandler:(NSString *)eventName handler:(::EventHandler)handler
{
	return lwf->AddEventHandler([eventName UTF8String], ^(Movie *, Button *) {
		handler();
	});
}

- (void)removeEventHandler:(NSString *)eventName handlerId:(int)handlerId
{
	lwf->RemoveEventHandler([eventName UTF8String], handlerId);
}

- (void)clearEventHandler:(NSString *)eventName
{
	lwf->ClearEventHandler([eventName UTF8String]);
}

- (void)clearAllEventHandlers
{
	lwf->ClearAllEventHandlers();
}

- (BOOL)inputPoint:(CGPoint)point
{
	Button *button = lwf->InputPoint(point.x, point.y);
	return button ? YES : NO;
}

- (void)inputPress
{
	lwf->InputPress();
}

- (void)inputRelease
{
	lwf->InputRelease();
}

- (void)play:(NSString *)target
{
	lwf->PlayMovie([target UTF8String]);
}

- (void)stop:(NSString *)target
{
	lwf->StopMovie([target UTF8String]);
}

- (void)nextFrame:(NSString *)target
{
	lwf->NextFrameMovie([target UTF8String]);
}

- (void)prevFrame:(NSString *)target
{
	lwf->PrevFrameMovie([target UTF8String]);
}

- (void)setVisible:(NSString *)target visible:(BOOL)visible
{
	lwf->SetVisibleMovie([target UTF8String], visible);
}

- (void)gotoAndStop:(NSString *)target frame:(int)frame
{
	lwf->GotoAndStopMovie([target UTF8String], frame);
}

- (void)gotoAndStop:(NSString *)target label:(NSString *)label
{
	lwf->GotoAndStopMovie([target UTF8String], [label UTF8String]);
}

- (void)gotoAndPlay:(NSString *)target frame:(int)frame
{
	lwf->GotoAndPlayMovie([target UTF8String], frame);
}

- (void)gotoAndPlay:(NSString *)target label:(NSString *)label
{
	lwf->GotoAndPlayMovie([target UTF8String], [label UTF8String]);
}

- (void)move:(NSString *)target x:(float)x y:(float)y
{
	lwf->MoveMovie([target UTF8String], x, y);
}

- (void)moveTo:(NSString *)target x:(float)x y:(float)y
{
	lwf->MoveToMovie([target UTF8String], x, y);
}

- (void)rotate:(NSString *)target degree:(float)degree
{
	lwf->RotateMovie([target UTF8String], degree);
}

- (void)rotateTo:(NSString *)target degree:(float)degree
{
	lwf->RotateToMovie([target UTF8String], degree);
}

- (void)scale:(NSString *)target x:(float)x y:(float)y
{
	lwf->ScaleMovie([target UTF8String], x, y);
}

- (void)scaleTo:(NSString *)target x:(float)x y:(float)y
{
	lwf->ScaleToMovie([target UTF8String], x, y);
}

- (void)setAlpha:(NSString *)target alpha:(float)alpha
{
	lwf->SetAlphaMovie([target UTF8String], alpha);
}

- (void)setColorTransform:(NSString *)target
	red:(float)red green:(float)green blue:(float)blue alpha:(float)alpha
{
	ColorTransform c(red, green, blue, alpha);
	lwf->SetColorTransformMovie([target UTF8String], &c);
}

- (BOOL)attachMovie:(NSString *)linkageName
	target:(NSString *)target attachName:(NSString *)attachName
{
	Movie *movie = lwf->SearchMovieInstance([target UTF8String]);
	if (!movie)
		return NO;

	Movie *attachedMovie =
		movie->AttachMovie([linkageName UTF8String], [attachName UTF8String]);
	if (!attachedMovie)
		return NO;

	return YES;
}

- (void *)lwf
{
	return lwf.get();
}

@end
