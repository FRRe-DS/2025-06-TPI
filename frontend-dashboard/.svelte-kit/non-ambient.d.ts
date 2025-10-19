
// this file is generated — do not edit it


declare module "svelte/elements" {
	export interface HTMLAttributes<T> {
		'data-sveltekit-keepfocus'?: true | '' | 'off' | undefined | null;
		'data-sveltekit-noscroll'?: true | '' | 'off' | undefined | null;
		'data-sveltekit-preload-code'?:
			| true
			| ''
			| 'eager'
			| 'viewport'
			| 'hover'
			| 'tap'
			| 'off'
			| undefined
			| null;
		'data-sveltekit-preload-data'?: true | '' | 'hover' | 'tap' | 'off' | undefined | null;
		'data-sveltekit-reload'?: true | '' | 'off' | undefined | null;
		'data-sveltekit-replacestate'?: true | '' | 'off' | undefined | null;
	}
}

export {};


declare module "$app/types" {
	export interface AppTypes {
		RouteId(): "/" | "/api" | "/api/shipments" | "/shipments" | "/shipments/[id]";
		RouteParams(): {
			"/shipments/[id]": { id: string }
		};
		LayoutParams(): {
			"/": { id?: string };
			"/api": Record<string, never>;
			"/api/shipments": Record<string, never>;
			"/shipments": { id?: string };
			"/shipments/[id]": { id: string }
		};
		Pathname(): "/" | "/api" | "/api/" | "/api/shipments" | "/api/shipments/" | "/shipments" | "/shipments/" | `/shipments/${string}` & {} | `/shipments/${string}/` & {};
		ResolvedPathname(): `${"" | `/${string}`}${ReturnType<AppTypes['Pathname']>}`;
		Asset(): "/robots.txt" | string & {};
	}
}