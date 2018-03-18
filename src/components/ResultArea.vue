<template>
    <v-layout>
      <v-flex>
        <div>
          <v-btn raised large color="error" v-on:click="resetList" :disabled="inputData.length < 1">Reset</v-btn>
          <v-btn depressed large color="success" v-on:click="recomputeResults" :disabled="inputData.length < 2">Compare</v-btn>
        </div>
        <v-tabs
          v-if="showTables"
          centered
          icons-and-text
          slot="extension"
          v-model="tabs"
          slider-color="blue darken-2"
          grow
        >
          <v-tab :href="`#tab-simple`">
            Simple table
            <v-icon large>fa-list</v-icon>
          </v-tab>
          <v-tab :href="`#tab-series`">
            By series
            <v-icon large>fa-tv</v-icon>
          </v-tab>
          <v-tab :href="`#tab-character`">
            By character
            <v-icon large>fa-users</v-icon>
          </v-tab>
        </v-tabs>
        <v-tabs-items v-model="tabs" v-if="showTables">
          <v-tab-item :id="`tab-simple`">
            <simple-table :inputData="inputData" :avatarMode="avatarMode" :recalculation="showTables" :counter="counter"></simple-table>
          </v-tab-item>
          <v-tab-item :id="`tab-series`">
            <series-table :inputData="inputData" :avatarMode="avatarMode" :recalculation="showTables" :counter="counter"></series-table>
          </v-tab-item>
          <v-tab-item :id="`tab-character`">
          </v-tab-item>
        </v-tabs-items>
      </v-flex>
    </v-layout>
</template>

<script>
import SimpleTable from '@/components/SimpleTable.vue'
import SeriesTable from '@/components/SeriesTable.vue'

export default {
  name: 'ResultArea',
  components: {
    'simple-table': SimpleTable,
    'series-table': SeriesTable
  },
  props: ['inputData'],
  data () {
    return {
      showTables: false,
      avatarMode: false,
      windowWidth: 0,
      counter: 0,
      tabs: 'tab-simple'
    }
  },
  methods: {
    resetList () {
      this.$emit('resetList')
    },
    handleResize (windowWidth) {
      if (windowWidth / this.inputData.length < 400) {
        this.avatarMode = true
      } else {
        this.avatarMode = false
      }
    },
    recomputeResults () {
      this.showTables = true
      this.counter++
    }
  },
  watch: {
    inputData: function (newVal, oldVal) {
      if (this.inputData.length === 0) {
        this.showTables = false
      }
    },
    windowWidth: function (newWidth, oldWidth) {
      this.handleResize(newWidth)
    }
  },
  mounted () {
    let that = this
    this.$nextTick(function () {
      window.addEventListener('resize', function (e) {
        that.windowWidth = window.innerWidth
        that.handleResize(this.windowWidth)
      })
    })
  },
  beforeDestroy: function () {
    window.removeEventListener('resize', this.handleResize)
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
