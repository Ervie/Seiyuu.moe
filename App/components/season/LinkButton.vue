<template>
  <nuxt-link
  :to="anotherSeasonPath">
    <v-btn
      large
      :loading="loadingAnotherSeason"
      :disabled="loadingAnotherSeason"
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

