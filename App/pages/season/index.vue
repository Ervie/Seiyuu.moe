<template>
    <form>
    <v-text-field
      v-model="year"
      :rules="yearErrors"
      :counter="4"
      label="Year"
      required
    ></v-text-field>
    <v-select
      v-model="season"
      :items="seasonItems"
      label="Season"
      required
    ></v-select>


      <nuxt-link :to="selectedSeasonPath">
    <v-btn nuxt color="secondary">
    Go
    </v-btn>
    </nuxt-link>
  </form>
</template>

<script>
export default {
  name: 'SeasonSearchPage',
  data() {
    return {
      valid: true,
      year: '2019',
      yearErrors: [
        v => !!v || 'Year is required.',
        v => (v && Number.isInteger(Number(v))) || 'Year must be a number.',
        v => (Number(v) < new Date().getFullYear() || Number(v) > 1917) || 'Year must be a number between 1917 and ' + new Date().getFullYear()
      ],
      season: 'Winter',
      seasonItems: [
        'Winter',
        'Spring',
        'Summer',
        'Fall'
      ]
    }
  },
  computed: {
    selectedSeasonPath() {
      return 'season/' + this.year + '/' + this.season.toLowerCase(); 
    }
  }
}
</script>
