if (typeof window !== "undefined") {
	/* Browsers */
	global = window;
} else if (typeof cc !== "undefined") {
	/* cocos2d native */
	global = cc;
}
