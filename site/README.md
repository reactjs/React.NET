ReactJS.NET Site
================

This is the website for ReactJS.NET. It consists of two parts:
 - The actual site, built using [Jekyll](http://jekyllrb.com/)
 - A package repository for development builds, using
   [Simple NuGet Server](https://github.com/daniel15/simple-nuget-server)
   (requires PHP)

To set up the site as it's configured in production:

```sh
# Install Nginx and HHVM (or PHP-FPM)
apt-get install nginx-full hhvm
# Install Nginx configuration
ln -s /var/www/reactjs.net/site/nginx.conf /etc/nginx/sites-enabled/reactjs.net && /etc/init.d/nginx reload
# Install Jekyll and other dependencies
gem install jekyll jekyll-assets bourbon
# Build the site
cd jekyll
jekyll build
```

When developing, you can run `jekyll serve -w` to start a server at
localhost:4000 and automatically rebuild when any files are changed.
