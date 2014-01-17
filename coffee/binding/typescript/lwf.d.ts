declare module LWF {

	export class Color {
		alpha:number;
		blue:number;
		green:number;
		red:number;

		constructor(r:number, g:number, b:number, a:number);
		constructor();

		set(r:number, g:number, b:number, a:number):void;
		set(c:Color):void;
	}

	export class ColorTransform {
		multi:Color;

		constructor(r:number, g:number, b:number, a:number);
		constructor();

		clear():void;
		set(c:ColorTransform):ColorTransform;
	}

	export class Matrix {
		scaleX:number;
		scaleY:number;
		skew0:number;
		skew1:number;
		translateX:number;
		translateY:number;

		constructor(sx:number, sy:number, s0:number, s1:number, tx:number, ty:number);
		constructor();

		clear():void;
		set(m:Matrix):Matrix;
	}

	export class Point {
		x:number;
		y:number;

		constructor(ax:number, ay:number);
		constructor();
	}

	export class StageSize {
		width:number;
		height:number;
	}

	export class Bounds {
		xMin:number;
		xMax:number;
		yMin:number;
		yMax:number;
	}

	export class Data {
		check():boolean;
		name():string;
	}

	export class Renderer {
		lwf:LWF;

		constructor(l:LWF);

		destruct():void;
		update(matrix:Matrix, colorTransform:ColorTransform):void;
		render(matrix:Matrix, colorTransform:ColorTransform, renderingIndex:number, renderingCount:number, visible:boolean):void;
	}

	export class ResourceCache {
		static get():ResourceCache;

		clear():void;
		loadLWF(settings:Object):void;
		loadLWFs(settingsArray:Object[], errors:Object[]):void;
		setParticleConstructor(name:string, ctor:Function):void;
		setDOMElementConstructor(name:string, ctor:Function):void;
	}

	export class LObject {
		isBitmapClip:boolean;
		isButton:boolean;
		isMovie:boolean;
		lwf:LWF;
		parent:Movie;
	}

	export class IObject extends LObject {
		name:string;

		getFullName():string;
	}

	export class BitmapClip extends LObject {
		alpha:number;
		depth:number;
		height:number;
		name:string;
		regX:number;
		regY:number;
		rotation:number;
		scaleX:number;
		scaleY:number;
		visible:boolean;
		width:number;
		x:number;
		y:number;

		addTween():Tween;
		setMatrix(matrix:Matrix):void;
		stopTween():void;
	}

	export class Button extends IObject {
		height:number;
		hitX:number;
		hitY:number;
		width:number;

		addEventListener(type:string, listener:Function):void;
		clearEventListener(type?:string):void;
		removeEventListener(type:string, listener:Function):void;
		setEventListener(type:string, listener:Function):void;
	}

	export class EventParams {
		type:string;
		param:any;
	}

	export class Movie extends IObject {
		active:boolean;
		alpha:number;
		blendMode:string;
		currentFrame:number;
		depth:number;
		playing:boolean;
		rotation:number;
		scaleX:number;
		scaleY:number;
		totalFrames:number;
		visible:boolean;
		x:number;
		y:number;

		constructor()

		addEventListener(type:string, listener:Function):void;
		addTween():Tween;
		attachBitmap(pLinkage:string, pDepth:number):BitmapClip;
		attachEmptyMovie(pName:string):Movie;
		attachLWF(pLwf:LWF, attachName:string, option?:Object):void;
		attachMovie(linkage:LWF, attachName:string, option?:Object):Movie;
		attachMovie(linkageName:string, attachName:string, option?:Object):Movie;
		clearEventListener(type?:string):void;
		detachBitmap(pDepth:number):void;
		detachFromParent():void;
		detachLWF(attachName:string):void;
		detachLWF(pDepth:number):void;
		detachLWF(pLwf:LWF):void;
		detachMovie(attachDepth:number):void;
		detachMovie(attachName:string):void;
		detachMovie(movieClip:Movie):void;
		dispatchEvent(e:EventParams):void;
		dispatchEvent(type:string):void;
		getAttachedBitmap(pDepth:number):BitmapClip;
		getAttachedBitmaps():BitmapClip[];
		getAttachedLWF(attachName:any):LWF;
		getAttachedLWF(pDepth:number):LWF;
		getAttachedMovie(attachName:any):Movie;
		getAttachedMovie(pDepth:number):Movie;
		getBounds():Bounds;
		globalToLocal(value:Point):Point;
		gotoAndPlay(frame:number):void;
		gotoAndPlay(labelName:string):void;
		gotoAndStop(frame:number):void;
		gotoAndStop(labelName:string):void;
		gotoFrame(frame:number):void;
		gotoLabel(label:string):void;
		inspect(pCallback:Function):void;
		localToGlobal( value:Point ):Point;
		move(x:number, y:number):Movie;
		moveTo(x:number, y:number):Movie;
		nextEnterFrame(pFunc:Function):void;
		nextFrame():void;
		play():void;
		prevFrame():void;
		removeEventListener(type:string, listener:Function):void;
		removeMovieClip():void;
		requestCalculateBounds(pCallback?:Function):void;
		rotate(degree:number):Movie;
		rotateTo(degree:number):Movie;
		scale(sx:number, sy:number):Movie;
		scaleTo(sx:number, sy:number):Movie;
		searchAttachedLWF(attachName:string, recursive?:boolean):LWF;
		searchAttachedMovie(attachName:string, recursive?:boolean):Movie;
		searchMovieInstance(instanceName:string):Movie;
		setAlpha(pAlpha:number):void;
		setColorTransform(colorTransform:ColorTransform):void;
		setEventListener(type:string, listener:Function):void;
		setMatrix(matrix:Matrix):void
		setPreferredFrameRate(pFrameRate:number, pSkipLimit:number):void;
		setRenderingOffset(offset:number):void;
		setVisible(pVisible:boolean):void;
		stop():void;
		stopTweens():void;
		swapAttachedLWFDepth(depth0:number, depth1:number):void;
		swapAttachedMovieDepth(depth0:number, depth1:number):void;
		swapDepths(depth:number):void;
		swapDepths(movie:Movie):void;
	}

	export class Property {
		clear():void;
		move(x:number, y:number):void;
		moveTo(x:number, y:number):void;
		rotate(v:number):void;
		rotateTo(v:number):void;
		scale(x:number, y:number):void;
		scaleTo(x:number, y:number):void;
		setAlpha(a:number):void;
		setColorTransform(c:ColorTransform):void;
		setMatrix(m:Matrix, sx?:number, sy?:number, r?:number):void;
	}

	export class LWF {
		attachName:string;
		depth:number;
		fastForward:boolean;
		fastForwardTimeout:number;
		frameRate:number;
		frameSkip:boolean;
		height:number;
		interactive:boolean;
		name:string;
		pointX:number;
		pointY:number;
		pressing:boolean;
		privateData:any;
		property:Property;
		rendererFactory:any;
		resourceCache:ResourceCache;
		rootMovie:Movie;
		stage:any;
		tick:number;
		time:number;
		width:number;

		static useCanvasRenderer():void;
		static useWebGLRenderer():void;
		static useWebkitCSSRenderer():void;

		addAllowButton(buttonName:string):boolean;
		addButtonEventListener(instanceName:string, listeners:Object):void;
		addDenyButton(buttonName:string):boolean;
		addEventListener(event:string, listener:Function):void;
		addMovieEventListener(instanceName:string, listeners:Object):void;
		clearAllowButton():void;
		clearButtonEventListener(instanceName:string, type?:string):void;
		clearDenyButton():void;
		clearEventListener(event:string):void;
		clearMovieEventListener(instanceName:string, type?:string):void;
		denyAllButtons():void;
		destroy():void;
		dispatchEvent(event:string, movie?:Movie, button?:Button):void;
		exec(tick?:number, matrix?:Matrix, colorTransform?:ColorTransform):void;
		fitForHeight(stageWidth:number, stageHeight:number):void;
		fitForWidth(stageWidth:number, stageHeight:number):void;
		forceExec(matrix?:Matrix, colorTransform?:ColorTransform):void;
		forceExecWithoutProgress(matrix?:Matrix, colorTransform?:ColorTransform):void;
		getStageSize():StageSize;
		getStringId(str:string):number;
		init():void;
		inputKeyPress(code:number):void;
		inputPoint(x:number, y:number):Button;
		inputPress():void;
		inputRelease():void;
		inspect(inspector:Function):void;
		removeAllowButton(buttonName:string):boolean;
		removeButtonEventListener(instanceName:string, listeners:Object):void;
		removeDenyButton(buttonName:string):boolean;
		removeEventListener(event:string, listener:Function):void;
		removeMovieEventListener(instanceName:string, listeners:Object):void;
		render():void;
		scaleForHeight(stageWidth:number, stageHeight:number):void;
		scaleForWidth(stageWidth:number, stageHeight:number):void;
		searchAttachedLWF(attachName:string):LWF;
		searchAttachedMovie(attachName:string):Movie;
		searchEventId(event:string):number;
		searchFrame(movie:Movie, label:string):number;
		searchProgramObjectId(name:string):number;
		setButtonEventListener(instanceName:string, listeners:Object):void;
		setEventListener(event:string, listener:Function):void;
		setFastForward(pFastForward:boolean):void;
		setFastForwardTimeout(pFastForwardTimeout:number):void;
		setFrameRate(pFrameRate:number):void;
		setFrameSkip(frameSkip:boolean):void;
		setMovieCommand(instanceNames:string[], cmd:Function):void;
		setMovieEventListener(instanceName:string, listeners:Object):void;
		setPreferredFrameRate(preferredFrameRate:number, execLimit?:number):void;
		setProgramObjectConstructor(name:string, ctor:Function):Renderer;
		setTextScale(textScale:number):void;
	}

	export class Tween {
		static Easing:TweenEasing;
		static Interpolation:TweenInterpolation;

		chain():Tween;
		delay(frame:number):Tween;
		easing(easeFunc:Function):Tween;
		interpolation(func:Function):Tween;
		onComplete(func:Function):Tween;
		onUpdate(func:Function):Tween;
		start():Tween;
		stop():Tween;
		to(params:Object, frame:number):Tween;
	}

	export interface TweenEasing {
		Linear:TweenEasingLinear;
		Quadratic:TweenEasingQuadratic;
		Cubic:TweenEasingCubic;
		Quartic:TweenEasingQuartic;
		Quintic:TweenEasingQuintic;
		Sinusoidal:TweenEasingSinusoidal;
		Exponential:TweenEasingExponential;
		Circular:TweenEasingCircular;
		Elastic:TweenEasingElastic;
		Back:TweenEasingBack;
		Bounce:TweenEasingBounce;
	}

	export interface TweenEasingLinear {
		None:Function;
	}

	export interface TweenEasingQuadratic {
		In:Function;
		Out:Function;
		InOut:Function;
	}

	export interface TweenEasingCubic {
		In:Function;
		Out:Function;
		InOut:Function;
	}

	export interface TweenEasingQuartic {
		In:Function;
		Out:Function;
		InOut:Function;
	}

	export interface TweenEasingQuintic {
		In:Function;
		Out:Function;
		InOut:Function;
	}

	export interface TweenEasingSinusoidal {
		In:Function;
		Out:Function;
		InOut:Function;
	}

	export interface TweenEasingExponential {
		In:Function;
		Out:Function;
		InOut:Function;
	}

	export interface TweenEasingCircular {
		In:Function;
		Out:Function;
		InOut:Function;
	}

	export interface TweenEasingElastic {
		In:Function;
		Out:Function;
		InOut:Function;
	}

	export interface TweenEasingBack {
		In:Function;
		Out:Function;
		InOut:Function;
	}

	export interface TweenEasingBounce {
		In:Function;
		Out:Function;
		InOut:Function;
	}

	export interface TweenInterpolation {
		Linear:Function;
		Bezier:Function;
		CatmullRom:Function;

		Utils:TweenInterpolationUtils;
	}

	export interface TweenInterpolationUtils {
		Linear:Function;
		Bernstein:Function;
		Factorial:Function;
		CatmullRom:Function;
	}

	export var _root:Movie;
}
