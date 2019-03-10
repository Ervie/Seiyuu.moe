<template>
  <v-layout row wrap>
      <v-flex xs12>
        <v-autocomplete
          :items="items"
          :search-input.sync="search"
          :loading="loading"
          v-model="model"
          dark
          hide-no-data
          label="Search by Seiyuu Name... (e.g. Kana Hanazawa)"
          item-text="name"
          item-value="malId"
          :menu-props="{maxHeight:'300'}"
          prepend-icon="search"
        >
          <template v-slot:item="data">
            <template v-if="typeof data.item !== 'object'">
              <v-list-tile-content v-text="data.item"></v-list-tile-content>
            </template>
            <template v-else>
              <v-list-tile-avatar>
                <v-img 
                  class="dropdownAvatar" 
                  :src="pathToImage(data.item.imageUrl)" />
              </v-list-tile-avatar>
              <v-list-tile-content>
                <v-list-tile-title v-html="data.item.name"></v-list-tile-title>
              </v-list-tile-content>
            </template>
          </template>
        </v-autocomplete>
      <alert-ribbon 
        :alerts="alerts" />
      </v-flex>
  </v-layout>
</template>

<script>
import axios from 'axios'
import seiyuu from 'static/quickSeiyuuList.json'
import AlertRibbon from '@/components/shared/ui-components/AlertRibbon.vue'

export default {
  name: 'SeiyuuBrowser',
  components: {
    'alert-ribbon': AlertRibbon
  },
  props: {
    searchedId: {
      type: Array,
      required: false
    }
  },
  data () {
    return {
      entries: [],
      loading: false,
      model: null,
      search: null,
      searchResults: [],
      timeout: null,
      timeoutLimit: 300,
      shareLinkData: null,
      maximumSeiyuuNumber: 6,
      alerts: [
        {
          name: 'tooMuchRecords',
          text: 'You can choose 6 seiyuu at max.',
          value: false
        },
        {
          name: 'alreadyOnTheList',
          text: 'This seiyuu is already selected.',
          value: false
        },
        {
          name: 'tooManyRequests',
          text: 'There are has too many requests to send. Wait a few seconds and select seiyuu again.',
          value: false
        },
        {
          name: 'dataUnobtainable',
          text: 'This data is currently not obtainable :(',
          value: false
        },
        {
          name: 'reloadNeeded',
          text: 'Network error occured during loading additional seiyuu list. Please consider refreshing the page.',
          value: false
        },
        {
          name: 'serviceUnavailable',
          text: 'The service is currently unavailable. Please come back later.',
          value: false
        }
      ]
    }
  },
  computed: {
    requestUrl() {
      return process.env.API_URL +
            '/api/Seiyuu/' +
            '?Page=0&PageSize=10&SortExpression=Popularity DESC'
    },
    items () {
      return this.entries.map(entry => {
        return Object.assign({}, entry, { malId: entry.malId, name: entry.name, imageUrl: entry.imageUrl })
      })
    }
  },
  methods: {
    sendSeiyuuRequest () {
      if (this.model) {
        if (this.searchedId.length >= this.maximumSeiyuuNumber) {
          this.handleBrowsingError('tooMuchRecords')
        } else {
          this.loading = true
          if (this.searchedId.includes(parseInt(this.model))) {
            this.handleBrowsingError('alreadyOnTheList')
            this.loading = false
          } else {
            axios.get(process.env.JIKAN_URL + 'person/' + String(this.model))
              .then((response) => {
                this.addToList(response.data)
              })
              .catch((error) => {
                console.log(error)
                this.handleBrowsingError('tooManyRequests')
                this.loading = false
              })
          }
        }
      }
    },
    loadCachedSeiyuu () {
      this.$axios.get(this.requestUrl)
        .then((response) => {
          this.cachedSeiyuu = response.data.payload.results;
        })
        .catch((error) => {
          console.log(error)
          this.$axios.get(process.env.API_URL2 + '/api/Seiyuu')
            .then((response) => {
              this.cachedSeiyuu = response.data.payload.results;
            })
            .catch((error) => {
              console.log(error)
              this.handleBrowsingError('serviceUnavailable');
              this.cachedSeiyuu = []
            })
        })
    },
    excludeFromSearchResults () {
      this.searchedId.forEach(id => {
        var index = this.entries.map(x => x.malId).indexOf(id);
        if (index > -1) {
          this.entries.splice(index, 1);
        }
      });
    },
    loadDataFromLink () {
      if (this.shareLinkData.length > 1 && this.shareLinkData.length < 6) {
        this.shareLinkData.forEach(element => {
          if (!this.searchedId.includes(element) && Number.parseInt(element) !== 'NaN' && Number.parseInt(element) > 0) {
            this.loading = true
            axios.get(process.env.JIKAN_URL + 'person/' + String(element))
              .then((response) => {
                this.addToList(response.data)
              })
              .catch((error) => {
                console.log(error)
                this.handleBrowsingError('tooManyRequests')
                this.loading = false
              })
          }
        })
      }
    },
    emitRunImmediately () {
      if (this.searchedId != null && this.shareLinkData != null) {
        if (this.searchedId.length === this.shareLinkData.length) {
          this.$emit('runImmediately')
        }
      }
    },
    handleBrowsingError (errorType) {
      var errorIndex = this.alerts.map(x => x.name).indexOf(errorType)
      if (errorIndex !== -1) {
        this.alerts[errorIndex].value = true;
      }
    },
    resetAlerts () {
      this.alerts.forEach(alert => {
        alert.value = false
      });
    },
    addToList (returnedData) {
      this.$emit('seiyuuReturned', returnedData);
      this.loading = false;
      this.model = null;
      this.resetAlerts();
    }
  },
  mounted () {
    if (this.$route.query !== null && !this.isEmpty(this.$route.query)) {
      if (this.$route.query.seiyuuIds != null) {
        this.shareLinkData = this.$route.query.seiyuuIds.split(';')
        this.loadDataFromLink()
      }
    }
  },
  watch: {
    model: {
      handler: 'sendSeiyuuRequest',
      immediate: true
    },
    searchedId: {
      handler: 'emitRunImmediately',
      immediate: true
    },
    search (val) {
      clearTimeout(this.timeout)
      var self = this
      this.timeout = setTimeout(function() {
        self.isLoading = true

        if (self.model != null || val === '' || val === null || val.length < 3) {
          self.entries = []
          return
        }

        var requestUrl = process.env.API_URL +
          '/api/seiyuu/' +
          '?Page=0&PageSize=10&SortExpression=Popularity DESC' +
          '&SearchCriteria.Name=' +
          String(val.replace('/', ' ')) 

        axios.get(requestUrl)
          .then(res => {
            if (res.data.payload.results.length > 0) {
              self.entries = res.data.payload.results
              self.excludeFromSearchResults()
            }
            self.isLoading = false
          })
          .catch(error => {
            console.log(error);
            this.handleBrowsingError('serviceUnavailable');
            self.isLoading = false;
          })
      }, this.timeoutLimit)
    }
  }
}
</script>