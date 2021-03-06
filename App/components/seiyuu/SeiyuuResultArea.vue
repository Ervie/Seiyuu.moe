<template>
    <v-layout>
      <v-flex>
        <v-expansion-panel popout>
          <v-expansion-panel-content>
            <div slot="header" class="title">Compare Options</div>
            <v-card>
              <v-layout row wrap>
                <v-flex xs12 md6 lg4>
                  <v-checkbox label="Group by series" v-model="groupBySeries" color="secondary"></v-checkbox>
                </v-flex>
                <v-flex xs12 md6 lg4>
                  <v-checkbox label="Main roles only" v-model="mainRolesOnly" color="secondary"></v-checkbox>
                </v-flex>
              </v-layout>
            </v-card>
          </v-expansion-panel-content>
        </v-expansion-panel>
        <div>
          <v-btn raised large color="error" class="optionButton" 
            @click="resetList" 
            :disabled="seiyuuIds.length < 1">Reset</v-btn>
          <v-btn depressed large color="primary" class="optionButton" 
            @click="showResults" 
            :disabled="seiyuuIds.length < 2 || loadingComparison"
            :loading="loadingComparison">Compare</v-btn>
          <v-btn depressed large color="secondary" class="optionButton" 
            @click="generateShareLink" 
            :disabled="!showTables || seiyuuIds.length < 2">Share Link</v-btn>
        </div>
        <v-tabs
          v-if="showTables"
          centered
          icons-and-text
          slot="extension"
          v-model="tabs"
          color="primary"
          slider-color="secondary"
          grow
        >
          <v-tab :href="`#tab-table`">
            Table
              <font-awesome-icon size="3x" :icon="['fa', 'table']"/>
          </v-tab>
          <v-tab :href="`#tab-calendar`">
            Timeline
              <font-awesome-icon size="3x" :icon="['fa', 'calendar-alt']"/>
          </v-tab>
        </v-tabs>
        <v-tabs-items v-model="tabs" v-if="showTables">
          <v-tab-item :value="`tab-table`" >
            <seiyuu-table-selection 
              :tableData="outputData" 
              :counter="counter"
              :loadingComparison="loadingComparison"
            />
          </v-tab-item>
          <v-tab-item :value="`tab-calendar`" >
            <seiyuu-timeline-container
              :timelineItems="outputData" />
          </v-tab-item>
        </v-tabs-items>
      </v-flex>
      <share-link-snackbar
        :showSnackbar="snackbar"
        @hideSnackBar="snackbar = false"/>
      <loading-dialog
        :isLoading="loadingComparison"/>
    </v-layout>
</template>

<script>
import axios from 'axios'
import SeiyuuTableSelection from '@/components/seiyuu/SeiyuuTableSelection.vue'
import SeiyuuTimelineContainer from '@/components/seiyuu/timeline/SeiyuuTimelineContainer.vue'
import ShareLinkSnackbar from '@/components/shared/ui-components/ShareLinkSnackbar.vue';
import LoadingDialog from '@/components/shared/ui-components/LoadingDialog.vue';

export default {
  name: 'SeiyuuResultArea',
  components: {
    'seiyuu-table-selection': SeiyuuTableSelection,
    'share-link-snackbar': ShareLinkSnackbar,
    'seiyuu-timeline-container': SeiyuuTimelineContainer,
    'loading-dialog': LoadingDialog
  },
  props: {
    seiyuuIds: {
      type: Array,
      required: false
    },
    runImmediately: {
      type: Boolean,
      required: true
    }
  },
  data () {
    return {
      showTables: false,
      counter: 0,
      tabs: 'tab-table',
      outputData: [],
      loadingComparison: false,
      timelineDates: [],
      groupBySeries: true,
      mainRolesOnly: false,
      snackbar: false
    }
  },
  methods: {
    resetList () {
      this.$emit('resetList')
    },
    showResults () {
      this.showTables = true;
      this.computeResults();
    },
    computeResults () {
      if (this.seiyuuIds.length >= 2 && this.showTables)
      {
        this.loadingComparison = true;
        this.outputData = [];
        axios.get(this.getSeiyuuCompareRequest())
          .then((response) => {
            if (response.data !== null) {
              this.outputData = response.data;
              this.loadingComparison = false;
            }
          })
          .catch((error) => {
            this.loadingComparison = false;
          })
      }
    },
    getSeiyuuCompareRequest() {

      var requestIUrl = process.env.apiUrl +
        '/api/seiyuu/Compare';
      
      requestIUrl += '?MainRolesOnly=' + this.mainRolesOnly;
      
      requestIUrl += '&GroupByFranchise=' + this.groupBySeries;

      this.seiyuuIds.forEach(element => {
        requestIUrl += '&SeiyuuMalIds=' + element;
      });
      
      return requestIUrl;
    },
    generateShareLink () {
      var idString = ''
      this.seiyuuIds.forEach(element => {
        idString += element + ',';
      });
      
      idString = idString.slice(0, -1);
      idString = this.encodeURL(this.seiyuuIds);

      var shareLink = process.env.baseUrl + $nuxt.$route.path.toLowerCase() + '?seiyuuIds=' + idString;
      this.$copyText(shareLink);
      this.snackbar = true;
    }
  },
  watch: {
    seiyuuIds: function (newVal, oldVal) {
      if (this.seiyuuIds.length === 0) {
        this.showTables = false;
      }
    },
    runImmediately: function (val) {
      if (val === true) {
        this.showResults();
      }
    },
    mainRolesOnly: {
      handler: 'computeResults',
      immediate: false
    },
    groupBySeries: {
      handler: 'computeResults',
      immediate: false
    }
  }
}
</script>

<style>
img.miniav {
    height: 98px;
    width: 63px;
}

img.av {
    height: 140px;
    width: 90px;
}

</style>
