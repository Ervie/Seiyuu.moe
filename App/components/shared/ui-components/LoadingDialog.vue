<template>
    <v-dialog
        v-model="isLoading"
        hide-overlay
        persistent
        width="300"
      >
        <v-card
          color="secondary"
          dark
        >
          <v-card-text>
            {{ message }}
            <v-progress-linear
              indeterminate
              color="white"
              class="mb-0"
              height="10"
            ></v-progress-linear>
          </v-card-text>
        </v-card>
      </v-dialog>
</template>

<script>
export default {
  name: 'LoadingDialog',
  props: {
    isLoading: {
      type: Boolean,
      required: true
    }
  },
  data () {
    return {
      message: 'Loading data, please stand by'
    }
  }
}
</script>
