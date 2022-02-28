FROM node:10.22.0 as build-deps
WORKDIR /
COPY / ./
EXPOSE 3000
CMD ["yarn", "start"]
