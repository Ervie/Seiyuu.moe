# SeiyuuInterlink

> A webpage searching for collaborate works between seiyuu.

## Build Setup

``` bash
# install dependencies
npm install

# serve with hot reload at localhost:9000
npm run dev

# build for production with minification
npm run build

# build for production and view the bundle analyzer report
npm run build --report
```

## Features

* Searching collaboration works between seiyuu (up to 6 at once).
    * Mode for every combination.
    * Mode for grouping by series.
    * Mode for grouping by character.
    * Mode for grouping by franchise (group by character + series).
* Precached list for fast searching by seiyuu name.
* Searching seiyuu by MAL id (if not on cached list).
* Details cards for extra information.

## To-Do List

* Move cached seiyuu to database at some point (with simple API).
* Enhance juxtaposition by emphasis on most frequent collaborations (more modes).
    * Filtering by main roles.
    * Partial results.
    * Charts?
* Export function (save and download a csv/json - formats to be decided)
* Deploy a demo.

## Special mentions

Project uses Jikan API, an unofficial MyAnimeList API (courtesy of Nekomata). Project can be found under this [link](https://github.com/jikan-me/jikan/)