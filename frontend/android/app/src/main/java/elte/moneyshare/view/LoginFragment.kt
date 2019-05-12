package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import elte.moneyshare.SharedPreferences

import elte.moneyshare.R
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.showAsDialog
import elte.moneyshare.viewmodel.LoginViewModel
import kotlinx.android.synthetic.main.fragment_login.*

class LoginFragment : Fragment() {

    private lateinit var viewModel: LoginViewModel

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_login, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        viewModel = ViewModelProviders.of(this).get(LoginViewModel::class.java)

        loginButton.setOnClickListener {
            viewModel.putLoginUser("test1@test.hu", "default") { response, error ->
            //viewModel.putLoginUser(emailEditText.text.toString(), passwordEditText.text.toString()) { response, error ->
                if(error == null) {
                    DialogManager.showInfoDialog(response, context)
                    activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, GroupsFragment())?.commit()
                } else {
                    DialogManager.showInfoDialog(error, context)
                }
            }

            viewModel.getUsers { users, error ->
                Log.d("LoginFragment:", "users: $users")
            }
        }

        registrationButton.setOnClickListener {
            activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, RegisterFragment())?.addToBackStack(null)?.commit()
        }
        forgottenPasswordButton.setOnClickListener {
            activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, ForgotPasswordFragment())?.addToBackStack(null)?.commit()
        }
    }
}