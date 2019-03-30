### Version 2.0 - 30th March 2019 (latest)

* Instead sending requests to external API, all data is requested to backend
* Reduced amount of data transferred
* Massive speed boost to searching anime/seiyuu.
* Removed "Too much requests" issue altogether.
* Added logger.
* Database frequently updated by background workers.

### Version 1.5 - 11th March 2019

* The amount of data transferred has been reduced.
* Less "Too much requests" errors.
* Performance improvement on anime searching
* Fixes
    * Loading images (for most most).
    * Handling error 503 (service not available).
    * Added sample placeholder for search controls.

### Version 1.4 - 22th February 2019

* Features
    * Three available table views.
    * Timeline view for seiyuu comparison.
    * New about and home pages.
    * Changelog in about page.
* Fixes
    * Auto re-render when changing "Main roles only" option.
    * Removed few errors from console.
    * Adjusted text display.

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

### Version 1.2 - 15th January 2019

* Features
    * Share links (after search).
* Fixes
    * Fixed seiyuu photo positioning in search list.
    * Massive speed boost for anime searching.
    * Adjusted application to work under Jikan REST API v3.

### Version 1.1 - 15th August 2018

* Initial version.
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