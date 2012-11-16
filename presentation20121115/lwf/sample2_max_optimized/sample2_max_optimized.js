global.LWF.Script = global.LWF.Script || {};
global.LWF.Script["sample2_max_optimized"] = function() {
	var LWF = global.LWF.LWF;
	var Loader = global.LWF.Loader;
	var Movie = global.LWF.Movie;
	var Property = global.LWF.Property;
	var Point = global.LWF.Point;
	var Matrix = global.LWF.Matrix;
	var Color = global.LWF.Color;
	var ColorTransform = global.LWF.ColorTransform;
	var Tween = global.LWF.Tween;
	var _root;

	var Script = (function() {function Script() {}

	Script.prototype["init"] = function() {
		_root = this;
	};

	Script.prototype["destroy"] = function() {
		_root = null;
	};

	Script.prototype["_root_100_1"] = function() {
		this.stop();
	};

	Script.prototype["_root_100_button_btn_press"] = function() {
		this.gotoAndPlay(1);
	};

	Script.prototype["lightning_motion_10_1"] = function() {
		this.gotoAndPlay(Math.floor(Math.random()*40+20));
	};

	return Script;

	})();

	return new Script();
};
