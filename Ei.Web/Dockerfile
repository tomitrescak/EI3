FROM mhart/alpine-node

COPY package.json /tmp/package.json
COPY yarn.lock /tmp/yarn.lock
RUN cd /tmp && yarn
RUN mkdir -p /app && cp -a /tmp/node_modules /app/

WORKDIR /app
COPY . /app

RUN yarn run build

#ENV ROOT_URL=http://hugo3.scem.uws.edu.au
#ENV EXPRESS_PORT=3000
#ENV MAIL_URL=smtps://godspaw%40gmail.com:Bailen@smtp.gmail.com

#RUN yarn && yarn cache clean
#RUN ./node_modules/.bin/tsc -p app/server
#EXPOSE 3000
#ENTRYPOINT ["/sbin/tini", "--"]
CMD ["yarn", "start"]