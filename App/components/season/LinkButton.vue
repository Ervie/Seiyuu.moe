<template>
  <nuxt-link
  :to="anotherSeasonPath"
  :class="{'isDisabled': isDisabled}"
  :event="isDisabled ? '' : 'click'">
    <v-btn
      large
      :loading="loadingAnotherSeason"
      :disabled="isDisabled"
      color="secondary"
      class="white--text"
      @click="loadAnotherSeason"
    >
      <font-awesome-icon v-if="!isNext" icon="arrow-left" color="white" />
      {{ seasonName + ' ' + seasonYear }}
      <font-awesome-icon v-if="isNext" icon="arrow-right" color="white" />
    </v-btn>
  </nuxt-link>
</template>

<script>
export default {
  props: {
    seasonName: {
      type: String,
      required: true
    },
    seasonYear: {
      type: Number,
      required: true
    },
    isNext: {
      type: Boolean,
      required: true
    }
  },
  data() {
    return {
      loadingAnotherSeason: false
    }
  },
  computed: {
    anotherSeasonPath() {
      return '/season/' + this.seasonYear + '/' + this.seasonName.toLowerCase(); 
    },
    isDisabled() {
      // for now; Think about better solution
      return this.loadingAnotherSeason || this.seasonYear >= 2020 || this.seasonYear <= 1916;
    }
  },
  methods: {
    loadAnotherSeason() {
      this.loadingAnotherSeason = true;
      this.$emit('loadingAnotherSeason');
    }
  },
}
</script>

<style>
  .isDisabled {
    cursor: default !important;
  }
</style>
