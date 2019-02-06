# Seiyuu.moe

> A webpage searching for collaborate works between seiyuu.

## Link

Current version can be found under this [link](https://seiyuu.moe).

## Build Setup

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

## Features

* Searching collaboration works between seiyuu (up to 6 at once).
    * Mode for grouping by series.
    * Mode for grouping by character.
    * Mode for grouping by franchise (group by character + series).
    * Main roles only filter.
    * Precached list for fast searching most popular seiyuu.
    * Details cards for extra information.
* Searching common seiyuu between anime (up to 6 titles at once).
    * Mode for grouping by seiyuu.
    * Details cards for extra information.

## Changelog

### Version 1.3 - 26th January 2019

* Features
    * New display mode for tables.
    * Possibility to choose between new and old display mode.
    * Updated seiyuu database.
* Fixes
    * Added small delay to avoid 429 (too much requests) error.
    * Fixed incorrect data in Seiyuu cards.
    * Clearing alerts on loading correct data.
    * Fixed sharelinks.
    * Remove anime from search results if already selected.

**[Read More](https://github.com/Ervie/Seiyuu.moe/blob/master/Changelog.md)**

### Roadmap

* New entry page.
* Current entry page as separate (about) page.
* Improvements within compare algorithm.
* Displaying additional information - main or supporting role.
* Small changes in table appearance (fill blank space).
* Enhance juxtaposition by emphasis on most important collaborations.
    * Partial results.
    * Statistics for most selected comparisons.
* Franchise search - similar to anime search, loading data from entire anime franchise rather than single anime.
* Character search - find if seiyuu of selected characters worked together on other works.
* Searching for Visual Novels/Games.
  
## Special mentions

Project uses Jikan API, an unofficial MyAnimeList API, courtesy of Nekomata (Irfan Dahir). Project can be found under this [link](https://github.com/jikan-me/jikan/).
<div>Website icon made by <a href="https://www.flaticon.com/authors/icon-monk" title="Icon Monk">Icon Monk</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a> is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0" target="_blank">CC 3.0 BY.</a></div>