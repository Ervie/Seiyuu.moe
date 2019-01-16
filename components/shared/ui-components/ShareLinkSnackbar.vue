<template>
    <v-snackbar
        v-model="showSnackbar"
        color="secondary"
        :timeout="3000"
        right top
      >
        Sharelink has been copied to the clipboard.
        <v-btn
          dark
          flat
          @click="snackbarClicked"
        >
          Close
        </v-btn>
      </v-snackbar>
</template>

<script>
export default {
  name: 'ShareLinkSnackbar',
  props: {
      showSnackbar: {
        type: Boolean,
        required: true
      }
  },
  methods: {
    snackbarClicked: function () {
      this.$emit('snackbarClicked')
    }
  },
}
</script>

