# Stage 1: Build the SvelteKit application
FROM node:20-alpine AS build

WORKDIR /app

# Copy package.json and package-lock.json (if exists) to install dependencies
# This step is optimized for Docker caching
COPY package*.json ./

# Install dependencies
RUN npm install
# Install SvelteKit static adapter
RUN npm install @sveltejs/adapter-static --save-dev

# Copy the rest of the frontend source code
COPY . .

# Build the SvelteKit application
# Ensure your svelte.config.js is configured to output to a 'build' directory
# or adjust this path accordingly if it outputs elsewhere (e.g., 'dist')
RUN npm run build

# Stage 2: Serve the application with Nginx
FROM nginx:stable-alpine AS final

# Copy custom Nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy the built SvelteKit application from the build stage
# Adjust '/app/dist' if your SvelteKit build output directory is different
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]