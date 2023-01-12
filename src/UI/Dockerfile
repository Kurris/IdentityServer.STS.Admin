FROM nginx
COPY ./dist/  /usr/share/nginx/html/
COPY ./nginx.conf /etc/nginx/nginx.conf
COPY ../../../../ssl/identity.isawesome.cn_nginx/identity.isawesome.cn.crt /etc/nginx/identity.isawesome.cn.crt
COPY ../../../../ssl/identity.isawesome.cn_nginx/identity.isawesome.cn.key /etc/nginx/identity.isawesome.cn.key
