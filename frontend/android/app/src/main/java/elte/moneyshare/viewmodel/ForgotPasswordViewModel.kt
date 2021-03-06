package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.SharedPreferences
import elte.moneyshare.model.APIClient

class ForgotPasswordViewModel : ViewModel() {

    fun putForgotPassword(email: String, completion: (response: String?, error: String?) -> Unit) {
        SharedPreferences.forgotPasswordEmail = email
        APIClient.getRepository().postForgotPassword(email) { response, error ->
            if(error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }
}