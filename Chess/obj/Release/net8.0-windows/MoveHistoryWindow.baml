   M S B A M L   `   `   `  ���� 	  Chess    Chess.MoveHistoryWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Move History��$	��450��$	��300�� � 	Resources &   System.Windows.Controls.DataGrid �� 
TargetType �� Property �� Value=  7System.Windows.Controls.Primitives.DataGridColumnHeader*  $System.Windows.Controls.DataGridCell��%�  '        '  �     '  J    �� "  .���  � �� �#333��  � ��
 �������  �@ GridLinesVisibility$
 None=���  �@ HeadersVisibility$ Column=���  � �� �#666��  � ��$ 1q��� "  .���  � �� �#444��  � ��
 �������  � ��$
 Bold"���  � ��$ Center=���  � ��$ 10q��� "  .���  � �� �#333��  � ��
 �������  � �� �#444��  � ��$ 0,0,1,1q���  � ��$ 10q�.��� ���#1E1E1E.+�  -   $��MoveHistoryDataGrid��   AutoGenerateColumns$ False��   CanUserAddRows$ False��   
IsReadOnly$
 True��$��10q�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn	   Header$
	 Move��
   Width5  /System.Windows.Controls.DataGridLengthConverter$
 *    
IsReadOnly$
 True��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *
MoveNumber+  $	 White��$
 * $
 True�� �� *	WhiteMove+  $	 Black��$
 * $
 True�� �� *	BlackMove+