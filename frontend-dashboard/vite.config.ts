import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';

export default defineConfig({
	plugins: [sveltekit()],
	server: {
		proxy: {
			'/api': {
				target: 'http://api:8080', // Corrected target for Docker Compose
				changeOrigin: true
			}
		}
	}
});
