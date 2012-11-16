global.LWF.Script = global.LWF.Script || {};
global.LWF.Script["LWF_sample_charaAnimation"] = function() {
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

	Script.prototype["_root_0_btn_btn_press"] = function() {
		//alert("!!!");
		  if (_root.isJumping == 0) {
				_root.isJumping = 1;
				_root.charaSpd=3;
				_root.btnMc.gotoAndPlay("on");
				_root.charaMc.gotoAndStop("jump");
			}
	};

	Script.prototype["_root_enterFrame"] = function() {
		if (_root.jumpEndFlg == 1) {
				_root.jumpEndFlg = 0;
				_root.isJumping = 0;
				_root.charaSpd=5;
				_root.charaMc.gotoAndStop("run");
			}
			_root.charaX+=_root.charaSpd;
			if(_root.charaX>(_root.stageW+35)){
				_root.charaX=-35;
			}
			_root.charaMc.x=_root.charaX;
	};

	Script.prototype["_root_load"] = function() {
		_root.isJumping = 0;
		_root.jumpEndFlg = 0;
		_root.charaX=-50;
		_root.charaY=240;
		_root.charaSpd=5;
		_root.stageW=320;
	};

	Script.prototype["_root_postLoad"] = function() {
		_root.charaMc.x=_root.charaX;
		_root.charaMc.y=_root.charaY;
	};

	Script.prototype["btn_all_0_2"] = function() {
		this.stop();
	};

	Script.prototype["chara_0_2"] = function() {
		this.stop();
	};

	Script.prototype["jump_14_1"] = function() {
		this.stop();
		_root.jumpEndFlg = 1;
	};

	Script.prototype["run_21_2"] = function() {
		this.gotoAndPlay("loop");
	};

	return Script;

	})();

	return new Script();
};
