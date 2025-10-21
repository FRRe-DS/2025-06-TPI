FROM node:20-alpine

WORKDIR /app

COPY package*.json ./
RUN npm install
RUN npm install @sveltejs/adapter-static --save-dev # Re-added

COPY . .

# Default Vite development server port
EXPOSE 5173

CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]