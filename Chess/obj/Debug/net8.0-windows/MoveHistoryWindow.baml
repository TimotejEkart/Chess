   M S B A M L   `   `   `  ����	  Chess    Chess.MoveHistoryWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Move History��5   	   $	��450��6   $	��300��6+   $	��450��67   $	��300��6G   $	��450��6V   $	��300��6f    � 	Resources 5      &   System.Windows.Controls.DataGrid �� 
TargetType �� Property �� Value=  7System.Windows.Controls.Primitives.DataGridColumnHeader*  $System.Windows.Controls.DataGridCell��%  '        '  /    '      �� 5   
   "  6   .�5      ��  � �� �#3336+   �� 5       � ��
 �����6+   �� 5	       �@ GridLinesVisibility$
 None=�64   �� 5
       �@ HeadersVisibility$ Column=�62   �� 5       � �� �#6666,   �� 5       � ��$ 1q�60   5      �� 5   
   "  6   .�5      ��  � �� �#4446+   �� 5       � ��
 �����6+   �� 5       � ��$
 Bold"�6+   �� 5       � ��$ Center=�6;   �� 5       � ��$ 10q�6(   5      �� 5   
   "  6   .�5      ��  � �� �#3336+   �� 5       � ��
 �����6+   �� 5       � �� �#4446,   �� 5       � ��$ 0,0,1,1q�60   �� 5       � ��$ 10q�6(   5      5      .��5      � ���#1E1E1E.+�5   
     -   5        $��MoveHistoryDataGrid��5         AutoGenerateColumns$ False��6.      CanUserAddRows$ False��6J      
IsReadOnly$
 True��6a   $��10q�6s      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5       ,  &System.Windows.Controls.DataGridColumn	   Header$
	 Move��6%   
   Width5  /System.Windows.Controls.DataGridLengthConverter$
 * 6R      
IsReadOnly$
 True��6\   1  +System.Windows.Controls.DataGridBoundColumn   Binding 63   �� *
MoveNumber+6\     5!      $	 White��6%   $
 * 6R   $
 True��6\    64   �� *	WhiteMove+6\     5"      $	 Black��6%   $
 * 6R   $
 True��6\    64   �� *	BlackMove+6\   5$      5%      5&      