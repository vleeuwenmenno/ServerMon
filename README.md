# ServerMon API

This is the main repository for the ServerMon project.

# API Documentation

Use insomnia to edit the documentation and export it back to json.

Store it in `docs/insomnia.json`.

View the [API Documentation](https://minty-media.github.io/ServerMon-api)

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
        proxy_pass         http://localhost:36676;

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
    proxy_pass http://localhost:36676;

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
ssh -R 5000:localhost:5000 -R 3000:localhost:3000 user@example.com

```