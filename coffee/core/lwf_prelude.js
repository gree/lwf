if (typeof global === "undefined" && typeof window !== "undefined") {
	/* Browsers */
	global = window;
}
