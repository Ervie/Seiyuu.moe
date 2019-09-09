<template>
  <v-layout row wrap>
    <v-flex xs12>
      <v-autocomplete
        :items="items"
        :search-input.sync="search"
        no-filter
        v-model="model"
        dark
        :hide-no-data="(search === null || search === '' || search.length < 3) || loadingSearch"
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
                class="dropdown-avatar"
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

import AlertRibbon from '@/components/shared/ui-components/AlertRibbon.vue'

export default {
  name: 'SeiyuuSearchPage',
  components: {
    'alert-ribbon': AlertRibbon
  },
  data() {
    return {
      entries: [],
      loadingSearch: false,
      model: null,
      search: null,
      timeout: null,
      timeoutLimit: 300,
      alerts: [
        {
          name: 'serviceUnavailable',
          text: 'The service is currently unavailable. Please come back later.',
          value: false
        }
      ]
    }
  },
  computed: {
    items () {
      return this.entries.map(entry => {
        return Object.assign({}, entry, { malId: entry.malId, name: entry.name, imageUrl: entry.imageUrl })
      })
    }
  },
  methods: {
    goToSeiyuuDetails () {
      if (this.model) {
        this.$router.push("/seiyuu/" + this.model);
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
    }
  },
  watch: {
    model: {
      handler: 'goToSeiyuuDetails',
      immediate: true
    },
    search (val) {
      clearTimeout(this.timeout)
      var self = this
      this.timeout = setTimeout(function() {
        self.loadingSearch = true

        if (self.model != null || val === '' || val === null || val.length < 3) {
          self.entries = []
          return
        }

        var requestUrl = process.env.apiUrl +
          '/api/seiyuu/' +
          '?Page=0&PageSize=10&SortExpression=Popularity DESC' +
          '&SearchCriteria.Name=' +
          String(val.replace('/', ' ')) 

        axios.get(requestUrl)
          .then(res => {
            if (res.data.payload.results.length > 0) {
              self.entries = res.data.payload.results
            }
            self.loadingSearch = false
          })
          .catch(error => {
            console.log(error);
            self.handleBrowsingError('serviceUnavailable');
            self.loadingSearch = false;
          })
      }, this.timeoutLimit)
    }
  },
}
</script>

