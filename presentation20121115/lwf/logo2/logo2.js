global.LWF.Script = global.LWF.Script || {};
global.LWF.Script["logo2"] = function() {
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

	Script.prototype["_root_postLoad"] = function() {
		var position = {x: 240, y: 160, rotation: 0};
			var tween0 = new Tween(this.logo, position)
				.to({x: 360, y: 80, rotation: 359}, 120)
				.delay(30)
				.easing(Tween.Easing.Elastic.InOut);
		
			var tween1 = new Tween(this.logo, position)
				.to({x: 240, y: 160, rotation: 0}, 120)
				.easing(Tween.Easing.Elastic.InOut);
				
			tween0.chain(tween1);
			tween1.chain(tween0);
			tween0.start();
		
	};

	return Script;

	})();

	return new Script();
};
