using System;
using System.Collections.Generic;
using System.Text;

namespace VkBot
{
    /// <summary>
    /// Additional answer options codes list. 
    /// </summary>
    [Flags]
    public enum AnswerFlags {
        /// <summary>
        /// Receive attachments.
        /// </summary>
        ReceiveAttachments = 2,

        /// <summary>
        /// Receive expanded set of events.
        /// </summary>
        ReceiveExpandedSetOfEvents = 8,

        /// <summary>
        /// Return pts (it's required for the messages.getLongPollHistory 
        /// method operation without a limit of the most recent 256 events).
        /// </summary>
        ReceivePts = 32,

        /// <summary>
        /// For the event with code 8 (a friend is online) extra data is returned 
        /// in the field $extra (learn more in the Event Structure section).
        /// </summary>
        ReceiveExtraData = 64,

        /// <summary>
        /// Return the parameter random_id with a message (random_id can be 
        /// transferred while sending messages using the messages.send method).
        /// </summary>
        ReceiveRandomId = 128
    }

    /// <summary>
    /// Each dialog has flags, which are values received by summing up 
    /// any of the following parameters. Flags are set only for community dialogs.
    /// </summary>
    [Flags]
    public enum DialogFlags {
        /// <summary>
        /// Important dialog.
        /// </summary>
        Important = 1,

        /// <summary>
        /// Dialog with a community reply.
        /// </summary>
        Answered = 2
    }

    /// <summary>
    /// Each element of the updates array represents its own an array
    /// that contains the event code in the first element and some
    /// set of fields with additional information depending
    /// on the type of event. 
    /// </summary>
    public enum LongPollMessageCodes {
        /// <summary>
        /// Replace message flags (FLAGS:=$flags). 
        /// </summary>
        ReplaceMessageFlags = 1,

        /// <summary>
        /// Install message flags (FLAGS|=$mask). 
        /// </summary>
        InstallMessageFlags = 2,

        /// <summary>
        /// Reset message flags (FLAGS&amp;=~$mask). 
        /// </summary>
        ResetMessageFlags = 3,

        /// <summary>
        /// Add a new message.
        /// </summary>
        AddMessage = 4,

        /// <summary>
        /// Read all incoming messages with $peer_id up to and including $local_id.
        /// </summary>
        ReadAllIncomingMessages = 6,

        /// <summary>
        /// Read all outgoing messages with $peer_id up to and including $local_id.
        /// </summary>
        ReadAllOutgoingMessages = 7,

        /// <summary>
        /// A friend $user_id is online. $extra is not 0, if flag 64 was passed in mode. 
        /// The low byte (remaining from the division into 256) of an extra number 
        /// contains the platform ID (ref. 7. Platforms). 
        /// </summary>
        FriendOnline = 8,

        /// <summary>
        /// A friend $user_id is offline ($flags is 0, if the user left the website
        /// (for example, by pressing Log Out) and 1, if offline is due to 
        /// timing out (for example, the away status)). 
        /// </summary>
        FriendOffline = 9,

        /// <summary>
        /// Reset dialog flags $peer_id. Corresponds to the operation 
        /// (PEER_FLAGS &= ~$flags). An event is returned only for 
        /// community messages. 
        /// </summary>
        ResetDialogFlags = 10,

        /// <summary>
        /// Replace dialog flags $peer_id. Corresponds to the operation 
        /// (PEER_FLAGS:= $flags). An event is returned only for
        /// community messages. 
        /// </summary>
        ReplaceDialogFlags = 11,

        /// <summary>
        /// Install dialog flags $peer_id. Corresponds to the operation 
        /// (PEER_FLAGS|= $flags). An event is returned only for 
        /// community messages. 
        /// </summary>
        InstallDialogFlags = 12,

        /// <summary>
        /// One of the parameters (content, topic) of the conversation 
        /// $chat_id was changed. $self — 1 or 0 (whether the change 
        /// was caused by the user). 
        /// </summary>
        ConversationChanged = 51,

        /// <summary>
        /// User $user_id began typing in the dialog. The event 
        /// should happen once in ~5 seconds during continuous
        ///  typing. $flags = 1. 
        /// </summary>
        UserTypingInDialog = 61,

        /// <summary>
        /// User $user_id began typing in the conversation $chat_id. 
        /// </summary>
        UserTypingInConversation = 61,

        /// <summary>
        /// User $user_id completed a call with the ID $call_id. 
        /// </summary>
        UserCall = 70,

        /// <summary>
        /// New unread messages counter in the left menu equals $count.
        /// </summary>
        CounterUpdate = 80,

        /// <summary>
        /// Notification settings changed where $peer_id is a chat/user’s ID, 
        /// $sound is either 1 (sound notifications on) or 0 (sound notifications off), 
        /// $disabled_until shows notifications disabled for a certain period 
        /// (-1: forever, 0: enabled, other: timestamp for when it should be switched on). 
        /// </summary>
        NotificationSettingsChanged = 114
    }

    /// <summary>
    /// Each message has a flag, which is a value received 
    /// by summing up any of the following parameters. 
    /// </summary>
    [Flags]
    public enum MessageFlags {
        /// <summary>
        /// Message is unread.
        /// </summary>
        Unread = 1,

        /// <summary>
        /// Message is outgoing.
        /// </summary>
        Outbox = 2,

        /// <summary>
        /// Message was answered.
        /// </summary>
        Replied = 4,

        /// <summary>
        /// Message is marked as important.
        /// </summary>
        Important = 8,

        /// <summary>
        /// Message sent via chat.
        /// </summary>
        Chat = 16,

        /// <summary>
        /// Message sent by a friend.
        /// </summary>
        Friends = 32,

        /// <summary>
        /// Message marked as "Spam".
        /// </summary>
        Spam = 64,

        /// <summary>
        /// Message was deleted.
        /// </summary>
        Deleted = 128,

        /// <summary>
        /// Message was user-checked for spam.
        /// </summary>
        Fixed = 256,

        /// <summary>
        /// Message has media content.
        /// </summary>
        Media = 512
    }

    /// <summary>
    /// If mode contains flag 64, then in messages with code 8 (friend is offline), 
    /// extra data $extra will return in the third field. From this you can get 
    /// the platform ID $platform_id = $extra &amp; 0xFF ( = $extra % 256), 
    /// from which the user got online. 
    /// </summary>
    [Flags]
    public enum PlatformFlags {
        /// <summary>
        /// Mobile website version or unidentified mobile app.
        /// </summary>
        Mobile = 1,

        /// <summary>
        /// Official app for iPhone.
        /// </summary>
        /// ReSharper disable once InconsistentNaming
        iPhone = 2,

        /// <summary>
        /// Official app for iPad.
        /// </summary>
        /// ReSharper disable once InconsistentNaming
        iPad = 3,

        /// <summary>
        /// Official app for Android.
        /// </summary>
        Android = 4,

        /// <summary>   
        /// Official app for Windows Phone.
        /// </summary>
        /// ReSharper disable once InconsistentNaming
        wPhone = 5,

        /// <summary>
        /// Official app for Windows 8.
        /// </summary>
        Windows = 6,

        /// <summary>
        /// Full website version or unidentified apps.
        /// </summary>
        Web = 7
    }

}
