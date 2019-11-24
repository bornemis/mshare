package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.text.Editable
import android.text.TextWatcher
import android.util.Patterns
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import elte.moneyshare.FragmentDataKeys
import elte.moneyshare.R
import elte.moneyshare.disable
import elte.moneyshare.enable
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.util.showToast
import elte.moneyshare.viewmodel.NewPasswordViewModel
import kotlinx.android.synthetic.main.fragment_login.*
import kotlinx.android.synthetic.main.fragment_new_password.*
import kotlinx.android.synthetic.main.fragment_new_password.passwordEditText
import kotlinx.android.synthetic.main.fragment_new_password.passwordTextInputLayout

class NewPasswordFragment : Fragment() {

    private lateinit var viewModel: NewPasswordViewModel
    private var token: String? = null

    fun passwordValidator(txt: String): String {
        var pwdError = ""
        val uppercaseRegex = """[A-Z]""".toRegex()
        val lowercaseRegex = """[a-z]""".toRegex()
        val numberRegex    = """[0-9]""".toRegex()
        context?.let {
            if(txt.length <6)
            {
                pwdError = it.getString(R.string.minimum_characters).plus('\n')
            }
            if(!txt.contains(uppercaseRegex))
            {
                pwdError = pwdError.plus(it.getString(R.string.uppercase_required).plus('\n'))
            }
            if(!txt.contains(lowercaseRegex))
            {
                pwdError = pwdError.plus(it.getString(R.string.lowercase_required).plus('\n'))
            }
            if(!txt.contains(numberRegex))
            {
                pwdError = pwdError.plus(it.getString(R.string.number_required).plus('\n'))
            }
        }

        return pwdError
    }

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        token = arguments?.getString(FragmentDataKeys.NEW_PASSWORD_TOKEN.value)
        return inflater.inflate(R.layout.fragment_new_password, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        viewModel = ViewModelProviders.of(this).get(NewPasswordViewModel::class.java)

        confirmPasswordButton.setOnClickListener {
            val password = passwordEditText.text.toString()
            var err = false
            val pwdError = passwordValidator(password)
            if (pwdError.length > 1) {
                passwordTextInputLayout.error = pwdError
                err = true
            }
            else {
                passwordTextInputLayout.error = null
            }
            if(!err)
            {
                token?.let {
                    viewModel.putNewPassword(passwordEditText.text.toString(), it) { _, error ->
                        if (error == null) {
                            context?.let { getString(R.string.new_password_updated).showToast(it) }
                        } else {
                            DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.AUTH_LOGIN, context), context)
                        }
                    }
                }
            }

        }
    }
}