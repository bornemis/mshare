package elte.moneyshare.view.viewholder

import android.support.constraint.ConstraintLayout
import android.support.v7.widget.RecyclerView
import android.view.View
import android.widget.CheckBox
import android.widget.TextView
import elte.moneyshare.R

class SelectMemberViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {

    var memberRootLayout: ConstraintLayout = itemView.findViewById(R.id.memberRootLayout)
    var memberNameTextView: TextView = itemView.findViewById(R.id.memberNameTextView)
    var memberSpendingEditText: TextView = itemView.findViewById(R.id.memberSpendingEditText)
    var currencyTextView : TextView = itemView.findViewById(R.id.currency_TextView)
    var selectedMemberCheckBox: CheckBox = itemView.findViewById(R.id.selectedMemberCheckBox)
}