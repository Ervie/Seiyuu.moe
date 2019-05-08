<template>
  <v-container>
    <v-form
      ref="form"
      v-model="valid">
    <v-text-field
      v-model="selectedYear"
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
        <v-btn nuxt color="primary" 
          :disabled="!valid">
        Go
        </v-btn>
      </nuxt-link>
      <v-btn color="error" 
        @click="resetForm">
        Current
      </v-btn>
    </v-form>
    <v-layout>
      <season-card v-for="(cardSeason, i) in cardSeasons" :key="i"
        :season="cardSeason"
      />
    </v-layout>
  </v-container>
</template>

<script>
import SeasonCard from '@/components/season/SeasonCard.vue';


export default {
  name: 'SeasonSearchPage',
  components: {
    'season-card': SeasonCard,
  },
  data() {
    return {
      valid: true,
      selectedYear: '2019',
      currentYear: '2019',
      yearErrors: [
        v => !!v || 'Year is required.',
        v => (v && Number.isInteger(Number(v))) || 'Year must be a number between 1917 and ' + new Date().getFullYear(),
        v => (Number(v) <= new Date().getFullYear() && Number(v) >= 1917) || 'Year must be a number between 1917 and ' + new Date().getFullYear()
      ],
      selectedSeason: 'Winter',
      currentSeason: 'Winter',
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
      return 'season/' + this.selectedYear + '/' + this.selectedSeason.toLowerCase(); 
    },
    cardSeasons() {
      return [
        { name: this.seasonItems[this.mod(this.seasonItems.indexOf(this.currentSeason) - 2, 4)], year: this.currentSeason === 'Winter' || this.currentSeason === 'Spring' ? this.currentYear - 1 : this.currentYear},
        { name: this.seasonItems[this.mod(this.seasonItems.indexOf(this.currentSeason) - 1, 4)], year: this.currentSeason === 'Winter' ? this.currentYear - 1 : this.currentYear},
        { name: this.currentSeason, year: this.currentYear},
        { name: this.seasonItems[this.mod(this.seasonItems.indexOf(this.currentSeason) + 1, 4)], year: this.currentSeason === 'Fall' ? this.currentYear + 1 : this.currentYear}
      ]
    }
  },
  methods: {
    resetForm() {
      var currentDate = new Date();
      this.currentYear = currentDate.getFullYear();
      this.currentSeason = this.seasonItems[Math.floor(currentDate.getMonth() / 3)];
      this.selectedYear = this.currentYear;
      this.selectedSeason = this.currentSeason
    }
  },
  mounted() {
    this.resetForm();
  }
}
</script>
