if (typeof global === "undefined" && typeof window !== "undefined") {
	/* Browsers */
	global = window;
} else if (typeof window === "undefined" && typeof self !== "undefined") {
	/* Workers */
	global = self;
} else if (typeof cc !== "undefined") {
	/* cocos2d native */
	global = cc;
}
