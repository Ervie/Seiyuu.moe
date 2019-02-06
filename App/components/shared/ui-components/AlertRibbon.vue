<template>
  <div>
    <v-alert v-for="(alert, i) in alerts" :key="i" dismissible color="error" v-model="alert.value">
      {{ alert.text }}
    </v-alert>
  </div>
</template>

<script>
export default {
    name: 'AlertRibbon',
    props: {
      alerts: {
          type: Array,
          required: true
      }
    }
}
</script>

