global.LWF.Script = global.LWF.Script || {};
global.LWF.Script["sample1_max_optimized"] = function() {
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

	Script.prototype["_root_84_1"] = function() {
		console.log("aaaaaaaaaa");
		this.stop();
	};

	Script.prototype["_root_84_button_btn_mc_press"] = function() {
		console.log("press");
		this.gotoAndPlay(1);
	};

	Script.prototype["card_0_1"] = function() {
		this.stop();
	};

	return Script;

	})();

	return new Script();
};
