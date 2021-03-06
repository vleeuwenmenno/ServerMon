# ServerMon

A Server logging tool to log machine metrics like CPU, Memory and swap states.

## Future features

 - Warnings to Discord/Slack when CPU/Memory/Swap is being used for x % for x minutes long.
 - Log backups to Amazon S3/Cloud services/WebDAV?
 - Disk status logging

# API Documentation

Documentation is auto generated by ASP.NET and can be accessed from `/swagger/v1/swagger.json`
You can use insomnia to view the documentation or alternatively you can use the manually created doc file located in `/docs/insomnia.json`.

When you contribute make sure to store it in `docs/insomnia.json`.

View the [API Documentation](https://vleeuwenmenno.github.io/ServerMon-api)

## To update the docs pages run: (Run inside docs/)
```
    insomnia-documenter --config insomnia.json
```
Note: make sure you have insomnia-documenter installed (Get it here: https://github.com/jozsefsallai/insomnia-documenter)

## To view the docs run:
```
    npx serve
```

# NGINX Configuration

This configuration can be added to `/etc/nginx/sites-available/default` and will reverse proxy to port 36676.
It should take the real IP back to the API as well in case we need to rate limit a specific IP.

```
server {
    server_name   example.com;
    location / {
        proxy_pass         https://localhost:36677;

        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
        proxy_set_header   X-Real-IP $remote_addr;
    }
}
```

# NGINX Configuration with Plesk

This is a little more complicated because Plesk screws up NGINX default config files.

1. Add your domain or subdomain to Plesk
2. Go to Plesk->DOMAIN->Apache & Nginx settings and change the settings below:
   - Disable Nginx as a proxy (untick):
3. Apply and Save these settings
4. In the same window, add the following to Additional Nginx directives: 
```
location / {
    proxy_pass         https://localhost:36677;

    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection keep-alive;
    proxy_set_header Host $host;
    proxy_cache_bypass $http_upgrade;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
    proxy_set_header X-Real-IP $remote_addr;
}
```

5. Make sure to enable SSL. !!! WARNING: disable any CloudFlare proxy !!!

# Service configuration

1. Add `ServerMon-api.service` to `/etc/systemd/system/` (For example `/etc/systemd/system/ServerMon-api.service`)
2. Copy the binaries with all its dependencies to `/opt/ServerMon-api/` or any other place (Make sure to update the .service file to match the new binaries path!)
3. Enable the service using `sudo systemctl enable ServerMon-api`
4. Start the service using `sudo systemctl start ServerMon-api`
5. Grab a :coffee:/:tea: and relax ~

# Working remotely?

You can pass through your debugging session to a nginx reverse proxy server to access it normally by using SSH.

```
ssh -R 36677:localhost:36677 -R 36676:localhost:36676 user@example.com

```