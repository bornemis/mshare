package elte.moneyshare.view.Adapter

import android.content.Context
import android.os.Bundle
import android.support.v7.widget.RecyclerView
import android.util.Log
import android.view.LayoutInflater
import android.view.ViewGroup
import android.widget.Toast
import elte.moneyshare.FragmentDataKeys
import elte.moneyshare.R
import elte.moneyshare.entity.FilteredUserData
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.view.GroupPagerFragment
import elte.moneyshare.view.MainActivity
import elte.moneyshare.view.viewholder.SearchResultViewHolder
import elte.moneyshare.viewmodel.AddMembersViewModel
import elte.moneyshare.viewmodel.GroupViewModel

class SearchResultsRecyclerViewAdapter(
    private val context: Context,
    var filteredUsers: ArrayList<FilteredUserData>,
    private val groupId: Int,
    private val memberInvitedListener: MemberInvitedListener,
    private val model: AddMembersViewModel
) : RecyclerView.Adapter<SearchResultViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): SearchResultViewHolder {
        val itemView = LayoutInflater.from(parent.context).inflate(R.layout.list_item_search_result, parent, false)
        return SearchResultViewHolder(itemView)
    }

    override fun getItemCount(): Int {
        return filteredUsers.size
    }

    interface MemberInvitedListener {
        fun onInvited()
    }

    override fun onBindViewHolder(holder: SearchResultViewHolder, position: Int) {
        val filteredUser = filteredUsers[position]
        Log.d("onBindViewHolder", "filteredUser = $filteredUser")
        holder.nameTextView.text = filteredUser.displayName
        holder.emailTextView.text = filteredUser.email

        if(model.currentGroupData?.members?.map {it.id}?.contains(filteredUser.id)!!) {
            println(filteredUser.id)
            holder.inviteButton.isEnabled = false
            holder.inviteButton.setText(R.string.added_member_button_label)
        } else {
            holder.inviteButton.isEnabled = true
            holder.inviteButton.setText(R.string.add_member_button_label)
        }

        holder.inviteButton.setOnClickListener {
            memberInvitedListener.onInvited()
            model.postMember(groupId, filteredUser.id) { response, error ->
                if (error == null) {
                    val fragment = GroupPagerFragment()
                    val args = Bundle()
                    groupId?.let {
                        args.putInt(FragmentDataKeys.GROUP_PAGER_FRAGMENT.value, it)
                    }
                    fragment.arguments = args
                    (context as MainActivity).supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, fragment)
                        ?.addToBackStack(null)?.commit()
                    (context as MainActivity).onBackPressed()
                    Toast.makeText(context, context.getString(R.string.member_added_to_group_successfully), Toast.LENGTH_SHORT).show()
                } else {
                    DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.GROUPS_ADD_MEMBER, context), context)
                }
            }
        }
    }
}