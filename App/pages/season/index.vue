<template>
    <v-form
      ref="form"
      v-model="valid">
    <v-text-field
      v-model="year"
      :rules="yearErrors"
      :counter="4"
      label="Year"
      required
    ></v-text-field>
    <v-select
      v-model="selectedSeason"
      :items="seasonItems"
      label="Season"
      required
    ></v-select>
      <nuxt-link :to="selectedSeasonPath"
      :disabled="!valid">
        <v-btn nuxt color="secondary" 
          :disabled="!valid">
        Go
        </v-btn>
      </nuxt-link>
      <v-btn color="success" 
        @click="resetForm">
        Current
      </v-btn>
  </v-form>
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
        v => (v && Number.isInteger(Number(v))) || 'Year must be a number between 1917 and ' + new Date().getFullYear(),
        v => (Number(v) <= new Date().getFullYear() && Number(v) >= 1917) || 'Year must be a number between 1917 and ' + new Date().getFullYear()
      ],
      selectedSeason: 'Winter',
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
      return 'season/' + this.year + '/' + this.selectedSeason.toLowerCase(); 
    }
  },
  methods: {
    resetForm() {
      var currentDate = new Date();
      this.selectedSeason = this.seasonItems[Math.floor(currentDate.getMonth() / 3)];
      this.year = currentDate.getFullYear();
    }
  },
  mounted() {
    this.resetForm();
  }
}
</script>
