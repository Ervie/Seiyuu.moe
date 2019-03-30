# Seiyuu.moe

> A webpage searching for collaborate works between seiyuu.

## Link

Current version can be found under this [link](https://seiyuu.moe).

## Build Setup

### App

``` bash
# install dependencies
npm install

# serve with hot reload at localhost:3000
npm run dev

# build for production with minification
npm run generate

# build and start with SSR at port 3000
npm run build
npm run start
```

Require npm, vue and nuxt installed.

### API

``` bash

# install dependencies
dotnet restore

# build project in Release configuration
dotnet build --configuration Release

# start project at port 5000
dotnet SeiyuuMoe.API.dll
```

Require .Net Core 2.2 installed.

## Features

* Searching collaboration works between seiyuu (up to 6 at once).
    * Mode for grouping by series.
    * Three separate display modes.
    * Main roles only filter.
    * Timeline view.
    * Precached list for fast searching most popular seiyuu.
    * Details cards for extra information.
* Searching common seiyuu between anime (up to 6 titles at once).
    * Three separate display modes.
    * Details cards for extra information.

## Changelog

### Version 2.0 - 30th March 2019 

* Instead sending requests to external API, all data is requested to backend.
* Reduced amount of data transferred.
* Massive speed boost to searching anime/seiyuu.
* Removed "Too much requests" issue altogether.
* Added logger.
* Database frequently updated by background workers.

**[Read More](https://github.com/Ervie/Seiyuu.moe/blob/master/Changelog.md)**

### Roadmap

Most planned features can be found in [Projects](https://github.com/Ervie/Seiyuu.moe/projects) page.
  
## Special mentions

Project uses Jikan API, an unofficial MyAnimeList API, courtesy of Nekomata (Irfan Dahir). Project can be found under this [link](https://github.com/jikan-me/jikan/).
<div>Website icon made by <a href="https://www.flaticon.com/authors/icon-monk" title="Icon Monk">Icon Monk</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a> is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0" target="_blank">CC 3.0 BY.</a></div>