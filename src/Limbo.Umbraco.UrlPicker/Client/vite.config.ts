import { defineConfig } from 'vite';

export default defineConfig({
	build: {
		lib: {
			entry: 'src/bundle.manifests.ts',
			formats: ['es'],
			fileName: 'limbo-urlpicker',
		},
		outDir: '../wwwroot/App_Plugins/Limbo.Umbraco.UrlPicker',
		emptyOutDir: true,
		sourcemap: true,
		rollupOptions: {
			external: [/^@umbraco/],
		},
	},
});
