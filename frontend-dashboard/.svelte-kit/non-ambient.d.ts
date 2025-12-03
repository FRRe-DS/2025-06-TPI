
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
		RouteId(): "/" | "/dashboard" | "/dashboard/shipments" | "/dashboard/shipments/create" | "/dashboard/shipments/[id]" | "/dashboard/shipments/[id]/status" | "/dashboard/transport-methods" | "/shipments" | "/shipments/[id]";
		RouteParams(): {
			"/dashboard/shipments/[id]": { id: string };
			"/dashboard/shipments/[id]/status": { id: string };
			"/shipments/[id]": { id: string }
		};
		LayoutParams(): {
			"/": { id?: string };
			"/dashboard": { id?: string };
			"/dashboard/shipments": { id?: string };
			"/dashboard/shipments/create": Record<string, never>;
			"/dashboard/shipments/[id]": { id: string };
			"/dashboard/shipments/[id]/status": { id: string };
			"/dashboard/transport-methods": Record<string, never>;
			"/shipments": { id?: string };
			"/shipments/[id]": { id: string }
		};
		Pathname(): "/" | "/dashboard" | "/dashboard/" | "/dashboard/shipments" | "/dashboard/shipments/" | "/dashboard/shipments/create" | "/dashboard/shipments/create/" | `/dashboard/shipments/${string}` & {} | `/dashboard/shipments/${string}/` & {} | `/dashboard/shipments/${string}/status` & {} | `/dashboard/shipments/${string}/status/` & {} | "/dashboard/transport-methods" | "/dashboard/transport-methods/" | "/shipments" | "/shipments/" | `/shipments/${string}` & {} | `/shipments/${string}/` & {};
		ResolvedPathname(): `${"" | `/${string}`}${ReturnType<AppTypes['Pathname']>}`;
		Asset(): "/robots.txt" | string & {};
	}
}