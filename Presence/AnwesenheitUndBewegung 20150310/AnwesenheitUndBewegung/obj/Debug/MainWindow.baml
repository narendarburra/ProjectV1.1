   M S B A M L   `   `   `  ����Q  MAnwesenheitUndBewegung, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:AnwesenheitUndBewegungAnwesenheitUndBewegung  5   	   '    !AnwesenheitUndBewegung.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   4src$clr-namespace:AnwesenheitUndBewegung     5   	   -   5          � Title$  Presence Detector��5   	   $	��300��6#   $	��500��60   .��5      � -   5        $��visGrid��5       � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $��2* �6   �� 5      $
��Auto �6   .+�5   
   �� $	��100��6   $��0��6   $��3��6,    �� SelectedIndex 6<   ��  �� Path$ UserId�.@�5      �� $��not logged in��6   �� 5      $��User1��6   �� 5      $��User2��6   �� 5      $��User3��6   5      �� 5   
   $��1��6   $��0��6"   .+�5      �� $��Center=�6   $��Presence��65   �� 5      -   5         ��Name$ presenceLabel��5      $��5,0,5,5q�6+   $��50��6<   $��50��6H   ��6S   �� 6l   5      �� 5!   
   $��2��6   $��0��6"   .+�5"      �� $��Center=�6   $��Movement��65   �� 5#      -   5        $ movementLabel��5#      $��5,0,5,5q�6+   $��50��6<   $��50��6H   ��6S   �� 6l   5$      �� 5'   
   $��20q�6   $��1��6    $��1��6-   ��6=   �� $ 
AbsentText��� 5(   
   $��20q�6   $��2��6    $��1��6-   ��6=   �� $ NoMovementText��� 5+   
   $��20q�6   $��1��6!   $��2��6.   $��2��6>    �� Orientation$ 
Horizontal=�6R   .+�5,      �� $��20��6   $	��200��6&   $	��Top=�62   $%��Allowed time absent in minutes:��6J   �� 5-      $��20��6   $	��Top=�6"   ��6:   �� $ AcceptedTimeAbsent�5.      �� 5/   
   $��20q�6   $��2��6!   $��2��6.   $��2��6>   $ 
Horizontal=�6R   .+�50      �� $��20��6   $	��200��6&   $	��Top=�62   $)��#Allowed time not moving in minutes:��6J   �� 51      $��20��6   $	��Top=�6#   ��6;   �� $ AcceptedTimeNotMoving�52      �� 54   
   $��20q�6   $��3��6!   $��2��6.   $��2��6>   $ 
Horizontal=�6R   .+�55      �� $��20��6   $	��200��6&   $	��Top=�62   $)��#Time until notification in minutes:��6J   �� 56      $��20��6   $	��Top=�6#   ��6;   �� $ TimeUntilNotification�57      �� 59   
   $��20q�6   $��3��6!   $��2��6.   .+�5:      �� ��6   �� $ WarningSentText��� 5;      ��6   �� $ NotificationSentText�5<      5=      5>      