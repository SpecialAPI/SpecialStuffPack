using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Feedback
{
	[Serializable]
	public class TrelloCardResponse
	{
		public string id;
		public Badges badges;
		public bool[] checkItemStates;
		public bool closed;
		public DateTime dateLastActivity;
		public string desc;
		public Descdata descData;
		public string due;
		public bool dueComplete;
		public string email;
		public string idBoard;
		public string[] idChecklists;
		public string[] idLabels;
		public string idList;
		public string[] idMembers;
		public int idShort;
		public string idAttachmentCover;
		public bool manualCoverAttachment;
		public CardLabel[] labels;
		public string name;
		public int pos;
		public string shortUrl;
		public string url;
		public string[] stickers;
	}

	[Serializable]
	public class Badges
	{
		public int votes;
		public bool viewingMemberVoted;
		public bool subscribed;
		public string fogbugz;
		public int checkItems;
		public int checkItemsChecked;
		public int comments;
		public int attachments;
		public bool description;
		public string due;
		public bool dueComplete;
	}

	[Serializable]
	public class Descdata
	{
		public Emoji emoji;
	}

	[Serializable]
	public class Emoji
	{
	}

	[Serializable]
	public class CardLabel
	{
		public string id;
		public string idBoard;
		public string name;
		public string color;
		public int uses;
	}
}
