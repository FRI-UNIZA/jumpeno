# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install dependencies
RUN apt-get update && \
    apt-get install -y python3 && \
    apt-get clean
RUN apt-get update && \
    curl -fsSL https://deb.nodesource.com/setup_22.x | bash - && \
    apt-get install -y nodejs && \
    apt-get clean
RUN apt-get update && apt-get install -y jq
RUN npm install -g clean-css-cli terser

# Copy project files
COPY ./Jumpeno.Client/Jumpeno.Client.csproj ./Jumpeno.Client/
COPY ./Jumpeno.Server/Jumpeno.Server.csproj ./Jumpeno.Server/
COPY ./Jumpeno.Shared/Jumpeno.Shared.csproj ./Jumpeno.Shared/
COPY ./Jumpeno.sln ./

# Restore workloads and dependencies
RUN dotnet workload restore ./Jumpeno.Client/Jumpeno.Client.csproj
RUN dotnet restore ./Jumpeno.sln

# Copy the entire source code
COPY ./Jumpeno.Client ./Jumpeno.Client
COPY ./Jumpeno.Server ./Jumpeno.Server
COPY ./Jumpeno.Shared ./Jumpeno.Shared
COPY ./Scripts ./Scripts
COPY ./Jumpeno.Shared/appsettings.json /app/appsettings.shared.json

# Minify static CSS & JS files based on the "Bundle" value
RUN BUNDLE=$(jq -r '.Bundle' ./Jumpeno.Shared/appsettings.json) && \
    if [ "$BUNDLE" = "true" ]; then \
        echo "Bundling static files..."; \
        find ./Jumpeno.Client/wwwroot/css -type f -name "*.css" -print0 | xargs -0 cleancss -o ./Jumpeno.Client/wwwroot/css/bundle.css && \
        find ./Jumpeno.Client/wwwroot/css -mindepth 1 ! -name "bundle.css" -exec rm -rf {} + && \
        sed -n '/<!-- Global JS -->/,/<!-- Global JS END -->/p' ./Jumpeno.Server/Pages/_Host.cshtml | \
            grep -oP '(?<=@AppEnvironment.Import\(")[^"]+(?="\))' | \
            sed 's/^/\.\/Jumpeno.Client\/wwwroot\//' | \
            tr '\n' '\0' | \
            grep -z -v './Jumpeno.Client/wwwroot/js/bundle.js' | \
            xargs -0 terser -o ./Jumpeno.Client/wwwroot/js/bundle.js --compress --mangle --keep-classnames --keep-fnames && \
        find ./Jumpeno.Client/wwwroot/js -mindepth 1 ! -name "bundle.js" -exec rm -rf {} +; \
    else \
        echo "Bundling disabled!"; \
    fi

# Publish the server
RUN dotnet publish Jumpeno.Server/Jumpeno.Server.csproj -c Release -o /app

# Stage 2: Use a runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "Jumpeno.Server.dll"]
